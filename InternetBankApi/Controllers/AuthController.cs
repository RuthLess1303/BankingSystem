using BankingSystemSharedDb.Db.Entities;
using BankingSystemSharedDb.Db.Repositories;
using BankingSystemSharedDb.Requests;
using InternetBankApi.Authorisation;
using InternetBankCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InternetBankApi.Controllers;

[ApiController]
[Route("api/authentification")]
public class AuthController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;

    public AuthController(
        TokenGenerator tokenGenerator,
        IUserService userService, 
        IUserRepository userRepository,
        UserManager<UserEntity> userManager)
    {
        _tokenGenerator = tokenGenerator;
        _userService = userService;
        _userRepository = userRepository;
        _userManager = userManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        await _userService.Register(request);
        
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        await _userService.Login(request);

        var roles = await _userManager.GetRolesAsync(user);
        return Ok(_tokenGenerator.Generate(user.Id.ToString(), roles));
    }
}