using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;
using ATMApplication.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATMTest.Services
{
    public class TransactionHistoryServiceTest
    {
        ATMContext context;

        ITransactionService transactionService;
        IAuthenticationService authenticationService;

        IRepository<Guid, Transaction> transactionRepo;
        IRepository<int, Account> accountRepo;
        IRepository<int, Card> cardRepo;

        [SetUp]
        public void Setup()
        {
            DbContextOptionsBuilder optionsBuilder = new DbContextOptionsBuilder().UseInMemoryDatabase("AuthenticationDB");
            context = new ATMContext(optionsBuilder.Options);
            transactionRepo = new TransactionRepository(context);
            accountRepo = new AccountRepository(context);
            cardRepo = new CardRepository(context);

            authenticationService = new AuthenticationService(cardRepo);
            transactionService = new TransactionService(transactionRepo, accountRepo, authenticationService);
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
            // Arrange
            Transaction transaction1 = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                Amount = 1000,
                Time = DateTime.Now,
                Type = TransactionTypeEnum.TransactionType.Deposit
            };
            Transaction transaction2 = new Transaction
            {
                Id = Guid.NewGuid(),
                AccountId = 1,
                Amount = 1000,
                Time = DateTime.Now,
                Type = TransactionTypeEnum.TransactionType.Withdrawal
            };
            await transactionRepo.Add(transaction1);
            await transactionRepo.Add(transaction2);

            await accountRepo.Add(new Account
            {
                AccountId = 1,
                CustomerID = 1,
                AccountNo = "AAA111",
                Balance = 3000
            });

            await cardRepo.Add(new Card
            {
                Id = 1,
                CardNumber = "ABC123",
                Pin = "9999",
                CustomerID = 1,
                CreatedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(3)
            });

            // Action
            var result = await transactionService.GetTransactionHistory(new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            });

            Assert.That(result.Count(), Is.EqualTo(2));

        }

        [Test]
        public async Task GetAllTransactionsFailTest()
        {
            // Arrange
            await cardRepo.Add(new Card
            {
                Id = 1,
                CardNumber = "ABC123",
                Pin = "9999",
                CustomerID = 1,
                CreatedDate = DateTime.Now,
                ExpiryDate = DateTime.Now.AddDays(3)
            });

            // Action
            var exception = Assert.ThrowsAsync<NoEntitiesFoundException>(() => transactionService.GetTransactionHistory(new AuthenticationDTO
            {
                CardNumber = "ABC123",
                Pin = "9999"
            }));
        }
    }
}