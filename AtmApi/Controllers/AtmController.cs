using AtmCore.Requests;
using AtmCore.Services;
using Microsoft.AspNetCore.Mvc;

namespace AtmApi.Controllers;

[ApiController]
[Route("api/atm")]
public class AtmController : Controller
{
    private readonly BalanceService _balanceService;
    private readonly PinService _pinService;
    private readonly WithdrawalService _withdrawalService;

    public AtmController(
        WithdrawalService withdrawalService,
        BalanceService balanceService,
        PinService pinService)
    {
        _withdrawalService = withdrawalService;
        _balanceService = balanceService;
        _pinService = pinService;
    }

    [Route("withdrawal")]
    [HttpPost]
    public async Task<OkResult> Withdrawal([FromBody] WithdrawalRequest request)
    {
        await _withdrawalService.Withdraw(request);
        return Ok();
    }

    [Route("balance")]
    [HttpPost]
    public async Task<OkObjectResult> SeeBalance([FromBody] WithdrawalRequest request)
    {
        var balance = await _balanceService.SeeBalance(request);
        return Ok(balance);
    }

    [Route("pin")]
    [HttpPut]
    public async Task<OkResult> ChangePin([FromBody] WithdrawalRequest request, string newPin)
    {
        await _pinService.ChangeCardPin(request, newPin);
        return Ok();
    }
}