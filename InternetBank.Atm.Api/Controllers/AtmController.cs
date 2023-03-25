using InternetBank.Atm.Core.Requests;
using InternetBank.Atm.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.Atm.Api.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController : Controller
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
    
    [HttpPost("withdraw")]
    public async Task<OkResult> Withdraw([FromBody]WithdrawalRequest request)
    {
        await _withdrawalService.Withdraw(request);
        return Ok();
    }
    
    [HttpPost("see-balance")]
    public async Task<OkObjectResult> SeeBalance([FromBody]AuthorizeCardRequest request)
    {
        var balance = await _balanceService.SeeBalance(request);
        return Ok(balance);
    }
    
    [HttpPost("change-pin")]
    public async Task<OkResult> ChangePin([FromBody]ChangePinRequest request)
    {
        await _cardPinService.ChangeCardPin(request);
        return Ok();
    }
}