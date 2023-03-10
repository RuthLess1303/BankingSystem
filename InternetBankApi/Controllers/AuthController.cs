using InternetBankApi.Authorisation;
using InternetBankCore.Db.Repositories;
using InternetBankCore.Requests;
using InternetBankCore.Services;
using Microsoft.AspNetCore.Mvc;
using MobileBank.Requests;

namespace InternetBankApi.Controllers;

[ApiController]
[Route("api/authentification")]
public class AuthController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;

    public AuthController(
        TokenGenerator tokenGenerator,
        IUserService userService, 
        IUserRepository userRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userService = userService;
        _userRepository = userRepository;
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
        var user = _userRepository.FindWithEmail(request.Email);
        if (user == null)
        {
            var operatorEntity = _userRepository.GetOperatorWithEmail(request.Email);

            return Ok(_tokenGenerator.Generate(operatorEntity.Id.ToString()));
        }
        await _userService.Login(request);

        return Ok(_tokenGenerator.Generate(user.Id.ToString()));
    }
}