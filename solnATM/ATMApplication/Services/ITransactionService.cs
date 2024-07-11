using ATMApplication.Models.DTOs;

namespace ATMApplication.Services
{
    public interface ITransactionService
    {
        Task<bool> Withdraw(WithdrawalDTO withdrawalDTO, int customerId);
    }
}
