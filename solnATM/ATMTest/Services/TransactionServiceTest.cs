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
        public async Task GetAllTransactionsSuccessTest()
        {

            Assert.Pass(); 
        }

        [Test]
        public async Task TestDeopsit()
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
    }
}
