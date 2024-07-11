using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;
using ATMApplication.Services;
using System.Security.Cryptography.X509Certificates;

namespace ATMApplication.Services
{
    public class TransactionService : ITransactionService
    {
        AuthenticationService authenticationService;
        IRepository<int, Customer> _customerRepository;
        IRepository<int, Account> _accountRepository; 
        IRepository<int, Transaction> _transactionRepository; 
        public TransactionService(IRepository<int , Card> cardRepo, IRepository<int , Customer>customerRepository , IRepository<int , Account>accountRepository, IRepository< int , Transaction>transactionRepository) {
            authenticationService = new AuthenticationService(cardRepo);
            _customerRepository = customerRepository;
            _accountRepository = accountRepository;
            _transactionRepository = transactionRepository;
        }
        public async Task<DepositReturnDTO> Deposit(DepositDTO depositDto)
        {
            try
            {
                int customerId = await authenticationService.AuthenticateCard(depositDto.authDetails); 
                var accounts = await _accountRepository.GetAll();
                var account = accounts.SingleOrDefault(x => x.CustomerID == customerId); 
                if (depositDto.amount > 20000)
                {
                    throw new DepositAmoutExceedExption(); 
                }
                if (account != null)
                {
                    Transaction t = new Transaction()
                    {
                        Amount = depositDto.amount,
                        Time = DateTime.Now,
                        Type=Transaction.TransactionType.Deposit,
                        AccountId=account.AccountId
                    };
                    await _transactionRepository.Add(t); 
                    account.Balance += depositDto.amount;
                    await _accountRepository.Update(account);
                    return new DepositReturnDTO
                    {
                        Success = true,
                        AccountNo = account.AccountNo,
                        CutomerId = customerId
                    }; 
                }
                else
                {
                    throw new EntityNotFoundException(); 
                }
            }
            catch
            {
                throw; 
            }
        }
    }
}
