﻿using ATMApplication.Exceptions;
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
                    int CustomerId = await _authenticationService.AuthenticateCard(authenticationDTO);
                    var result = await _transactionService.GetTransactionHistory(CustomerId);
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
    }
}
