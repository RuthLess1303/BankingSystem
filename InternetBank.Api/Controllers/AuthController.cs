using InternetBank.Api.Authorisation;
using InternetBank.Core.Services;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Db.Repositories;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.Api.Controllers;

[ApiController]
[Route("api/authentification")]
public class AuthController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserService _userService;
    private readonly UserManager<UserEntity> _userManager;
    private readonly ILoginLoggerRepository _loginLoggerRepository;

    public AuthController(
        TokenGenerator tokenGenerator,
        IUserService userService,
        UserManager<UserEntity> userManager, 
        ILoginLoggerRepository loginLoggerRepository)
    {
        _tokenGenerator = tokenGenerator;
        _userService = userService;
        _userManager = userManager;
        _loginLoggerRepository = loginLoggerRepository;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]LoginRequest request)
    {
        var user = await _userService.Login(request);
        var roles = await _userManager.GetRolesAsync(user);
        if (roles.Count == 0)
        {
            throw new Exception("Role for user was not found");
        }

        await _loginLoggerRepository.AddLoggedInUser(user.Id);

        var token = _tokenGenerator.Generate(user.Id.ToString(), roles);
        return Ok(token);
    }
}
