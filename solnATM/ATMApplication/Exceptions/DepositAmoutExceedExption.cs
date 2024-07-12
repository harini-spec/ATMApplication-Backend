using System.Runtime.Serialization;

namespace ATMApplication.Exceptions
{
    [Serializable]
    public class DepositAmoutExceedExption : Exception
    {
        public DepositAmoutExceedExption()
        {
        }

        public DepositAmoutExceedExption(string? message) : base(message)
        {
        }

        public DepositAmoutExceedExption(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DepositAmoutExceedExption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}