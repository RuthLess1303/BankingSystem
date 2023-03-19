using AtmCore.Requests;
using AtmCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtmApi.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController : Controller
{
    private readonly IBalanceService _balanceService;
    private readonly IPinService _pinService;
    private readonly IWithdrawalService _withdrawalService;

    public AtmController(
        IWithdrawalService withdrawalService,
        IBalanceService balanceService,
        IPinService pinService)
    {
        _withdrawalService = withdrawalService;
        _balanceService = balanceService;
        _pinService = pinService;
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
        await _pinService.ChangeCardPin(request);
        return Ok();
    }
}