using BeatBreeze.API.Dtos.Models;
using BeatBreeze.API.Utils;
using BeatBreeze.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeatBreeze.API.Controllers;

[Authorize]
public sealed class UsersController : BaseController
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var userId = User.GetUserGuid();

        var user = await _userService.GetUserById(userId);

        if (user is null)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var dto = UserDto.Create(user);

        return Ok(dto);
    }
}
