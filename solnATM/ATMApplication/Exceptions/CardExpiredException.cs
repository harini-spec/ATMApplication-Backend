using System.Runtime.Serialization;

namespace ATMApplication.Exceptions
{
    public class CardExpiredException : Exception
    {
        public CardExpiredException()
        {
        }

        public CardExpiredException(string? message) : base(message)
        {
        }
    }
}