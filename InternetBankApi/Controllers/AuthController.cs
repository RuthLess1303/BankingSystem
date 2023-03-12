using InternetBankApi.Authorisation;
using InternetBankCore.Db.Entities;
using InternetBankCore.Db.Repositories;
using InternetBankCore.Requests;
using InternetBankCore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InternetBankApi.Controllers;

[ApiController]
[Route("api/authentification")]
public class AuthController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IUserService _userService;
    private readonly UserManager<UserEntity> _userManager;
    private readonly RoleManager<RoleEntity> _roleManager;

    public AuthController(
        TokenGenerator tokenGenerator,
        IUserService userService,
        IUserRepository userRepository,
        UserManager<UserEntity> userManager,
        RoleManager<RoleEntity> roleManager)
    {
        _tokenGenerator = tokenGenerator;
        _userService = userService;
        _userRepository = userRepository;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
    {
        await _userService.Register(request);

        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
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