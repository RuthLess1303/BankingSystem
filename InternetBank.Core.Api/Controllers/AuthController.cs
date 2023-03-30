using InternetBank.Core.Api.Authorisation;
using InternetBank.Core.Services;
using InternetBank.Db.Db.Entities;
using InternetBank.Db.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace InternetBank.Core.Api.Controllers;

[ApiController]
[Route("api/authentification")]
public class AuthController : ControllerBase
{
    private readonly TokenGenerator _tokenGenerator;
    private readonly IUserService _userService;
    private readonly UserManager<UserEntity> _userManager;

    public AuthController(
        TokenGenerator tokenGenerator,
        IUserService userService,
        UserManager<UserEntity> userManager)
    {
        _tokenGenerator = tokenGenerator;
        _userService = userService;
        _userManager = userManager;
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
        
        return Ok(_tokenGenerator.Generate(user.Id.ToString(), roles));
    }
}