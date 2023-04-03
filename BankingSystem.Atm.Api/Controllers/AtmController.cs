using BankingSystem.Atm.Core.Requests;
using BankingSystem.Atm.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace BankingSystem.Atm.Api.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController : ControllerBase
{
    private readonly IBalanceService _balanceService;
    private readonly ICardPinService _cardPinService;
    private readonly IWithdrawalService _withdrawalService;

    public AtmController(
        IWithdrawalService withdrawalService,
        IBalanceService balanceService,
        ICardPinService cardPinService)
    {
        _withdrawalService = withdrawalService;
        _balanceService = balanceService;
        _cardPinService = cardPinService;
    }
    
    [HttpPost("Withdraw")]
    public async Task<IActionResult> Withdraw([FromBody]WithdrawalRequest request)
    {
        await _withdrawalService.Withdraw(request);
        return Ok();
    }
    
    [HttpGet("See_Balance")]
    public async Task<IActionResult> SeeBalance([FromBody]AuthorizeCardRequest request)
    {
        var balance = await _balanceService.SeeBalance(request);
        return Ok(balance);
    }
    
    [HttpPut("Change_Pin")]
    public async Task<IActionResult> ChangePin([FromBody]ChangePinRequest request)
    {
        await _cardPinService.ChangeCardPin(request);
        return Ok();
    }
}