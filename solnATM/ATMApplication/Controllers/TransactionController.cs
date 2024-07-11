using ATMApplication.Exceptions;
using ATMApplication.Models.DTOs;
using ATMApplication.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace ATMApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly IAuthenticationService _authenticationService;

        public TransactionController(ITransactionService transactionService, IAuthenticationService authenticationService)
        {
            _transactionService = transactionService;
            _authenticationService = authenticationService;
        }

        [HttpPost("withdraw")]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawalDTO withdrawalDTO)
        {
            try
            {
                // Validate the model state
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Extract authentication details
                var cardNumber = withdrawalDTO.AuthDetails.CardNumber;
                var pin = withdrawalDTO.AuthDetails.Pin;

                // Authenticate the card
                var customerId = await _authenticationService.AuthenticateCard(new AuthenticationDTO
                {
                    CardNumber = cardNumber,
                    Pin = pin
                });

                // Perform the withdrawal
                bool result = await _transactionService.Withdraw(new WithdrawalDTO
                {
                    Amount = withdrawalDTO.Amount // Only pass the amount to the transaction service
                }, customerId);

                return Ok(new { success = result });
            }
            catch (InvalidCredentialsException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (EntityNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
