using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Dtos.Requests;
using Microsoft.AspNetCore.Authentication;
using CloudMusicPlayer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser(CreateUserRequest createRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userCreationResult = await _authService.CreateUser(HttpContext, createRequest.Email, createRequest.Name, createRequest.Password);

        if (userCreationResult.IsFailure)
        {
            return BadRequest(userCreationResult.Error);
        }

        return Ok(UserDto.Create(userCreationResult.Value));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var signInResult = await _authService.SignIn(HttpContext, loginRequest.Email, loginRequest.Password);

        if (signInResult.IsFailure)
        {
            return Unauthorized("Your email or password is incorrect");
        }

        return Ok(UserDto.Create(signInResult.Value));
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}
