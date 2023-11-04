using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public sealed class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var user = await _userService.GetUserById(userIdResult.Value);

        if (user.IsFailure || user.Value is null)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var dto = UserDto.Create(user.Value);

        return Ok(dto);
    }
}
