using InternetBank.Core.Services;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.Api.Controllers;

[ApiController]
[Route("api/user")]
[Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
public class UserController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;
    private readonly ICardService _cardService;

    public UserController(
        ITransactionService transactionService, 
        IAccountService accountService, ICardService cardService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
        _cardService = cardService;
    }

    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("make-transaction")]
    public async Task<IActionResult> MakeTransaction([FromBody]TransactionRequest request)
    {
        await _transactionService.MakeTransaction(request);
        
        return Ok();
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-account")]
    public async Task<IActionResult> SeeAccount(string iban)
    {
        var account = await _accountService.SeeAccount(iban);
        var text = $"Your Balance is: {account.Item1}\nTransactions\n";

        if (account.Item2 != null)
        {
            foreach (var transaction in account.Item2)
            {
                text += $"{_transactionService.PrintTransaction(transaction)}\n";
            }
        }
        else
        {
            text += "There are no transactions made yet";
        }

        return Ok(text);
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-card")]
    public async Task<IActionResult> SeeCard(string iban)
    {
        var cardModel = await _cardService.SeeCard(iban);
        var cardInfo = _cardService.PrintCardModelProperties(cardModel);

        return Ok(cardInfo);
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-all-cards")]
    public async Task<string> SeeAllCards(string privateNumber)
    {
        var cards = await _cardService.SeeAllCards(privateNumber);
        var text = _cardService.PrintAllCardModelProperties(cards);

        return text;
    }
    
}