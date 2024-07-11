using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;
using System.Linq;
using System.Threading.Tasks;

namespace ATMApplication.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<int, Account> _accountRepo;
        private readonly IRepository<Guid, Transaction> _transactionRepo;

        public TransactionService(IRepository<int, Account> accountRepo, IRepository<Guid, Transaction> transactionRepo)
        {
            _accountRepo = accountRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<bool> Withdraw(WithdrawalDTO withdrawalDTO, int customerId)
        {
            var accounts = await _accountRepo.GetAll();
            var account = accounts.SingleOrDefault(a => a.Id == customerId);

            if (account == null)
            {
                throw new EntityNotFoundException("Account not found!");
            }

            if (withdrawalDTO.Amount > 10000)
            {
                throw new InvalidOperationException("Cannot withdraw more than 10000 in one transaction.");
            }

            if (account.Balance < withdrawalDTO.Amount)
            {
                throw new InvalidOperationException("Insufficient balance.");
            }

            account.Balance -= withdrawalDTO.Amount;
            await _accountRepo.Update(account);

            var transaction = new Transaction
            {
                Id = Guid.NewGuid(),
                Amount = withdrawalDTO.Amount,
                Time = DateTime.Now,
                Type = Transaction.TransactionType.Withdrawal,
                AccountId = account.AccountId
            };

            await _transactionRepo.Add(transaction);

            return true;
        }
    }
}
