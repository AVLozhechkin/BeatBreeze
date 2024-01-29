using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Filters;
using Microsoft.AspNetCore.Authentication;
using CloudMusicPlayer.API.Services;
using Microsoft.AspNetCore.Mvc;
using CloudMusicPlayer.API.Dtos.Models;

namespace CloudMusicPlayer.API.Controllers;

public sealed class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("create")]
    [ModelValidation]
    public async Task<UserDto> CreateUser(CreateUserRequest createRequest)
    {
        ArgumentNullException.ThrowIfNull(createRequest);

        var user = await _authService.CreateUser(HttpContext, createRequest.Email, createRequest.Password);
        return UserDto.Create(user);
    }

    [HttpPost("login")]
    [ModelValidation]
    public async Task<UserDto> Login(LoginRequest loginRequest)
    {
        ArgumentNullException.ThrowIfNull(loginRequest);

        var user = await _authService.SignIn(HttpContext, loginRequest.Email, loginRequest.Password);
        return UserDto.Create(user);
    }

    [HttpGet("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return Ok();
    }
}
