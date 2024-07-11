using ATMApplication.Models.DTOs;

namespace ATMApplication.Services
{
    public interface ITransactionService
    {
        public Task<DepositReturnDTO> Deposit(DepositDTO depositDto); 
    }
}
