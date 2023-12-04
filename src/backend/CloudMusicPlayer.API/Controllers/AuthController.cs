using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Filters;
using Microsoft.AspNetCore.Authentication;
using CloudMusicPlayer.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

public sealed class AuthController : BaseController
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("create-user")]
    [ModelValidation]
    public async Task CreateUser(CreateUserRequest createRequest)
    {
        await _authService.CreateUser(HttpContext, createRequest.Email, createRequest.Password);
    }

    [HttpPost("login")]
    [ModelValidation]
    public async Task<UserDto> Login(LoginRequest loginRequest)
    {
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
