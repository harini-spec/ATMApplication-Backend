using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;

namespace ATMApplication.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IRepository<Guid, Transaction> _transactionRepo;
        private readonly IRepository<int, Account> _accountRepo;
        public TransactionService(IRepository<Guid, Transaction> transactionRepo, IRepository<int, Account> accountRepo)
        {
            _transactionRepo = transactionRepo;
            _accountRepo = accountRepo;
        }

        public async Task<List<ReturnTransactionDTO>> GetTransactionHistory(int CustomerId)
        {
            var AllTransactions = await _transactionRepo.GetAll();
            var transactions = new List<Transaction>();
            foreach(var transaction in AllTransactions)
            {
                var account = await _accountRepo.GetById(transaction.AccountId);
                if(account.CustomerID == CustomerId)
                    transactions.Add(transaction);
            }
            if(transactions.Count == 0)
            {
                throw new NoEntitiesFoundException("No transactions found!");
            }
            var result = new List<ReturnTransactionDTO>();
            foreach (var transaction in transactions)
            {
                result.Add(await MapTransactionToReturnTransactionDTO(transaction));
            }
            return result;
        }

        private async Task<ReturnTransactionDTO> MapTransactionToReturnTransactionDTO(Transaction transaction)
        {
            ReturnTransactionDTO returnTransactionDTO = new ReturnTransactionDTO();
            returnTransactionDTO.Id = transaction.Id;
            var account = await _accountRepo.GetById(transaction.AccountId);
            returnTransactionDTO.AccountNo = account.AccountNo;
            returnTransactionDTO.Amount = transaction.Amount;
            returnTransactionDTO.Time = transaction.Time;
            returnTransactionDTO.Type = transaction.Type.ToString();
            return returnTransactionDTO;
        }
    }
}
