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

    public OperatorController(IUserService userService)
    {
        _userService = userService;
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
}