using ATMApplication.Models.DTOs;

namespace ATMApplication.Services
{
    public interface ITransactionService
    {
        public Task<List<ReturnTransactionDTO>> GetTransactionHistory(AuthenticationDTO authenticationDTO);
        public Task<DepositReturnDTO> Deposit(DepositDTO depositDto); 
    }
}
