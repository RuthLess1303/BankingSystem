using BankingSystemSharedDb.Requests;
using InternetBankCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBankApi.Controllers;

[ApiController]
[Route("api/user")]
public class UserController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly IAccountService _accountService;

    public UserController(
        ITransactionService transactionService, 
        IAccountService accountService)
    {
        _transactionService = transactionService;
        _accountService = accountService;
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
    public async Task<string> SeeAccount(string iban)
    {
        var account = await _accountService.SeeAccount(iban);
        var text = $"Your Balance is: {account.Item1}\n" +
                   $"Transactions\n";
        Parallel.ForEach(account.Item2, transaction =>
        {
            text += $"{transaction}\n";
        });
        
        return text;
    }
    
    [Authorize("ApiUser", AuthenticationSchemes = "Bearer")]
    [HttpPost("see-card")]
    public async Task<string> SeeCard(string iban)
    {
        var card = await _accountService.SeeCard(iban);
        var text = $"Your Card Information: {card.Item1}\n";
        if (card.Item2 == null)
        {
            return text;
        }
        text += $"Card Status: {card.Item2}";
        
        return text;
    }
}