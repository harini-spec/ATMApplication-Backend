using ATMApplication.Exceptions;
using ATMApplication.Models;
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
        private readonly ITransactionService _transactionService;
        private readonly IAuthenticationService _authenticationService;

        public TransactionController(ITransactionService transactionService, IAuthenticationService authenticationService)
        {
            _transactionService = transactionService;
            _authenticationService = authenticationService;
        }

        [HttpPost("GetAllTransactions")]
        [ProducesResponseType(typeof(List<ReturnTransactionDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]

        public async Task<ActionResult<List<ReturnTransactionDTO>>> GetAllTransactions(AuthenticationDTO authenticationDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _transactionService.GetTransactionHistory(authenticationDTO);
                    return Ok(result);
                }
                catch (NoEntitiesFoundException nef)
                {
                    return NotFound(new ErrorModel(404, nef.Message));
                }
                catch (EntityNotFoundException enf)
                {
                    return NotFound(new ErrorModel(404, enf.Message));
                }
                catch (Exception ex)
                {
                    return BadRequest(new ErrorModel(500, ex.Message));
                }
            }
            return BadRequest("All details are not provided. Please check the object");
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
