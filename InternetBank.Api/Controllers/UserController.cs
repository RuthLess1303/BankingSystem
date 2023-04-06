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
        await _currentUserValidation.IsSameUserWithIban(iban);
        var text = await _accountService.SeeAccount(iban);

        return Ok(text);
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-card")]
    public async Task<IActionResult> SeeCard(string iban)
    {
        await _currentUserValidation.IsSameUserWithIban(iban);
        var cardModel = await _cardService.SeeCard(iban);
        var cardInfo = _cardService.TurnCardInfoToJson(cardModel);

        return Ok(cardInfo);
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-all-cards")]
    public async Task<IActionResult> SeeAllCards(string privateNumber)
    {
        await _currentUserValidation.IsSameUserWithPrivateNumber(privateNumber);
        var cards = await _cardService.SeeAllCards(privateNumber);
        var cardInfo = _cardService.TurnCardInfoToJson(cards);

        return Ok(cardInfo);
    }
    
}