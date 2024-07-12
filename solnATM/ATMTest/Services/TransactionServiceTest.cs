using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;
using ATMApplication.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ATMTest.Services
{
    public class TransactionServiceTest
    {
        ATMContext context;
        ITransactionService transactionService;
        IRepository<Guid, Transaction> transactionRepo;
        IRepository<int, Account> accountRepo;
        IRepository<int, Card> cardRepo; 

        [SetUp]
        public async Task Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("AuthenticationDB");
            context = new ATMContext(optionsBuilder.Options);
            transactionRepo = new TransactionRepository(context);
            cardRepo = new CardRepository(context);
            accountRepo = new AccountRepository(context);
            IAuthenticationService authenticationService;
            authenticationService = new AuthenticationService(cardRepo); 
            transactionService = new TransactionService(transactionRepo, accountRepo, authenticationService);
            await context.Database.EnsureCreatedAsync(); 
            Account a = new Account()
            {
                AccountId = 1 , 
                AccountNo="1234", 
                Balance=1000, 
                CustomerID=1 , 
            }; 
            await accountRepo.Add(a);
        }

        [TearDown]
        public void TearDown()
        {
            context.Database.EnsureDeleted();
            context.Dispose();
        }

        [Test]
        public async Task TestDeposit()
        {
            DepositDTO depositDTO = new DepositDTO();
            AuthenticationDTO authenticationDTO = new AuthenticationDTO();
            authenticationDTO.CardNumber = "ABC123";
            authenticationDTO.Pin = "9999";
            depositDTO.authDetails = authenticationDTO; 
            depositDTO.amount = 1000;
            DepositReturnDTO res =   await transactionService.Deposit(depositDTO); 
            Assert.IsNotNull(res);
        }
        [Test]
        public async Task TestDepositThrowsCredientalsNotValidException()
        {
            DepositDTO depositDTO = new DepositDTO();
            AuthenticationDTO authenticationDTO = new AuthenticationDTO();
            authenticationDTO.CardNumber = "ABC123";
            authenticationDTO.Pin = "0000";
            depositDTO.authDetails = authenticationDTO; 
            depositDTO.amount = 1000;
            Assert.ThrowsAsync<InvalidCredentialsException>(async () => await transactionService.Deposit(depositDTO)); 
        }
        [Test]
        public async Task TestDepositThrowsDepositAmountExcedeedException() { 
            DepositDTO depositDTO = new DepositDTO();
            AuthenticationDTO authenticationDTO = new AuthenticationDTO();
            authenticationDTO.CardNumber = "ABC123";
            authenticationDTO.Pin = "9999";
            depositDTO.authDetails = authenticationDTO; 
            depositDTO.amount = 50000;
            Assert.ThrowsAsync<DepositAmoutExceedExption>(async () => await transactionService.Deposit(depositDTO)); 
        }

        [Test]
        public async Task TestDepositThrowsEntityNotFoundException()
        {
            await accountRepo.Delete(1);
            DepositDTO depositDTO = new DepositDTO();
            AuthenticationDTO authenticationDTO = new AuthenticationDTO();
            authenticationDTO.CardNumber = "ABC123";
            authenticationDTO.Pin = "9999";
            depositDTO.authDetails = authenticationDTO;
            depositDTO.amount = 5000;
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await transactionService.Deposit(depositDTO));
        }

        [Test]
        public async Task WithdrawSuccessTest()
        {
            // Arrange
            AuthenticationDTO authenticationDTO = new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            };
            WithdrawalDTO withdrawalDTO = new WithdrawalDTO
            {
                Amount = 100,
                AuthDetails = authenticationDTO
            };

            // Action
            var result = await transactionService.Withdraw(withdrawalDTO, 1);
            
            // Assert
            Assert.That(result, Is.EqualTo(true));
        }

        [Test]
        public async Task WithdrawNoAccountFailTest()
        {
            // Arrange
            await accountRepo.Delete(1);
            AuthenticationDTO authenticationDTO = new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            };
            WithdrawalDTO withdrawalDTO = new WithdrawalDTO
            {
                Amount = 100,
                AuthDetails = authenticationDTO
            };

            // Action
            Assert.ThrowsAsync<EntityNotFoundException>(async () => await transactionService.Withdraw(withdrawalDTO, 1));
        }

        [Test]
        public async Task WithdrawAmountLimitExceededFailTest()
        {
            // Arrange
            AuthenticationDTO authenticationDTO = new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            };
            WithdrawalDTO withdrawalDTO = new WithdrawalDTO
            {
                Amount = 15000,
                AuthDetails = authenticationDTO
            };

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await transactionService.Withdraw(withdrawalDTO, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Cannot withdraw more than 10000 in one transaction."));
        }

        [Test]
        public async Task WithdrawAmountExceedsBalanceFailTest()
        {
            // Arrange
            AuthenticationDTO authenticationDTO = new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            };
            WithdrawalDTO withdrawalDTO = new WithdrawalDTO
            {
                Amount = 5000,
                AuthDetails = authenticationDTO
            };

            // Action
            var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => await transactionService.Withdraw(withdrawalDTO, 1));

            // Assert
            Assert.That(exception.Message, Is.EqualTo("Insufficient balance."));
        }
    }
}
