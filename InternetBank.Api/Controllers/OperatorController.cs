using InternetBank.Core.Services;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.Api.Controllers;

[ApiController]
[Route("api/operator")]
[Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
public class OperatorController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IAccountService _accountService;
    private readonly ICardService _cardService;

    public OperatorController(
        IUserService userService,
        IAccountService accountService, 
        ICardService cardService)
    {
        _userService = userService;
        _accountService = accountService;
        _cardService = cardService;
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("register-user")]
    public async Task<IActionResult> RegisterUser([FromBody]RegisterUserRequest request)
    {
        await _userService.Register(request);
        
        return Ok();
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("create-account")]
    public async Task<IActionResult> CreateAccount([FromBody]CreateAccountRequest request)
    {
        await _accountService.CreateAccount(request);
        
        return Ok();
    }
    
    [Authorize("ApiOperator", AuthenticationSchemes = "Bearer")]
    [HttpPost("create-card")]
    public async Task<IActionResult> CreateCard([FromBody]CreateCardRequest request)
    {
        await _cardService.CreateCard(request);
        
        return Ok();
    }
}