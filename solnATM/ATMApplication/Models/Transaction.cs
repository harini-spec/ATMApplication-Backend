using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ATMApplication.Models
{
    public class Transaction
    {
        public enum TransactionType
        {
            Withdrawal,
            Deposit
        }

        [Key]
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public DateTime Time { get; set; }
        public TransactionType Type { get; set; }

        public string AccountNo { get; set; }
        [ForeignKey("AccountNo")]
        public Account Account { get; set; }
    }
}
