using ATMApplication.Exceptions;
using ATMApplication.Models.DTOs;
using ATMApplication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ATMApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        ITransactionService _transactionService;
        public TransactionController (ITransactionService transactionService)
        {
            this._transactionService = transactionService;
        }

        [HttpPost]
        [Route("Deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public  async Task<ActionResult<DepositReturnDTO>> DepositAmount(DepositDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return StatusCode(StatusCodes.Status400BadRequest);
            }
            try
            {
               DepositReturnDTO returndto =  await _transactionService.Deposit(dto);
                return returndto;
            }
            catch (EntityNotFoundException)
            {
                return StatusCode(StatusCodes.Status400BadRequest); 
            }
            catch (InvalidCredentialsException)
            {
                return StatusCode(StatusCodes.Status401Unauthorized);
            }
            catch (DepositAmoutExceedExption)
            {
                return StatusCode(StatusCodes.Status409Conflict); 
            }
            catch
            {
                return StatusCode(statusCode: StatusCodes.Status500InternalServerError); 
            }
        }
    }
}
