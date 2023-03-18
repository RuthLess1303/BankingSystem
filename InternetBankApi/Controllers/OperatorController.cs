using BankingSystemSharedDb.Requests;
using InternetBankCore.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBankApi.Controllers;

[ApiController]
[Route("api/operator")]
public class OperatorController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ICurrencyService _currencyService;

    public OperatorController(IUserService userService, ICurrencyService currencyService)
    {
        _userService = userService;
        _currencyService = currencyService;
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
    {
        await _userService.Register(request);
        
        return Ok();
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
    {
        await _userService.CreateAccount(request);
        
        return Ok();
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("create-card")]
    public async Task<IActionResult> CreateCard(CreateCardRequest request)
    {
        await _userService.CreateCard(request);
        
        return Ok();
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("add-currency-in-db")]
    public async Task<IActionResult> AddCurrencyInDb()
    {
        await _currencyService.AddInDb();
        
        return Ok();
    }
}