using System.Runtime.Serialization;

namespace ATMApplication.Exceptions
{
    public class DepositAmoutExceedExption : Exception
    {
        public DepositAmoutExceedExption()
        {
        }

        public DepositAmoutExceedExption(string? message) : base(message)
        {
        }
    }
}