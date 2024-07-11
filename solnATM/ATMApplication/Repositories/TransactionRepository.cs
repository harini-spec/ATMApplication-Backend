using ATMApplication.Models;

namespace ATMApplication.Repositories
{
    public class TransactionRepository : AbstractRepositoryClass<int, Transaction>
    {
        public TransactionRepository(ATMContext context) : base(context)
        {
        }
    }
}
