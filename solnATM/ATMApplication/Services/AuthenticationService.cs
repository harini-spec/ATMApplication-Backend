using ATMApplication.Exceptions;
using ATMApplication.Models;
using ATMApplication.Models.DTOs;
using ATMApplication.Repositories;

namespace ATMApplication.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        public readonly IRepository<int, Card> _cardRepo;
        public AuthenticationService(IRepository<int, Card> cardRepo)
        {
            _cardRepo = cardRepo;
        }

        public async Task<int> AuthenticateCard(AuthenticationDTO authenticationDTO)
        {
            var cards = await _cardRepo.GetAll();
            var card = cards.SingleOrDefault(c => c.CardNumber == authenticationDTO.CardNumber);
            if(card == null)
            {
                throw new InvalidCredentialsException("Invalid Credentials");
            }
            if(card.Pin == authenticationDTO.Pin)
            {
                return card.CustomerID;
            }
            throw new InvalidCredentialsException("Invalid Credentials");
        }

    }
}