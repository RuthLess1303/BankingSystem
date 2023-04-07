using InternetBank.Core.Services;
using InternetBank.Core.Validations;
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
    private readonly ICurrentUserValidation _currentUserValidation;

    public UserController(
        ITransactionService transactionService, 
        IAccountService accountService, 
        ICardService cardService, 
        ICurrentUserValidation currentUserValidation)
    {
        _transactionService = transactionService;
        _accountService = accountService;
        _cardService = cardService;
        _currentUserValidation = currentUserValidation;
    }
    
    [HttpPost("transactions")]
    public async Task<IActionResult> MakeTransaction([FromBody]TransactionRequest request)
    {
        await _transactionService.MakeTransaction(request);
        
        return Ok();
    }
    
    [HttpGet("accounts/{iban}")]
    public async Task<IActionResult> GetAccount(string iban)
    {
        await _currentUserValidation.IsSameUserWithIban(iban);
        var text = await _accountService.SeeAccount(iban);

        return Ok(text);
    }
    
    [HttpGet("cards/{iban}")]
    public async Task<IActionResult> GetCard(string iban)
    {
        await _currentUserValidation.IsSameUserWithIban(iban);
        var cardModel = await _cardService.SeeCard(iban);
        var cardInfo = _cardService.TurnCardInfoToJson(cardModel);

        return Ok(cardInfo);
    }
    
    [HttpPost("cards/all")]
    public async Task<IActionResult> GetAllCards()
    {
        var loggedUserPrivateNumber = await _currentUserValidation.GetLoggedUserPrivateNumber();
        var cards = await _cardService.SeeAllCards(loggedUserPrivateNumber);
        var cardInfo = _cardService.TurnCardInfoToJson(cards);

        return Ok(cardInfo);
    }
    
}