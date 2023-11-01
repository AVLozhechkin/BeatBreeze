using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistoriesController : ControllerBase
{
    private readonly HistoryService _historyService;

    public HistoriesController(HistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<ActionResult<History>> GetUserHistory()
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var historyResult = await _historyService.GetUserHistory(userIdResult.Value);

        if (historyResult.IsFailure)
        {
            return BadRequest(historyResult.Error);
        }

        return Ok(historyResult.Value);
    }

    [HttpPost("add")]
    public async Task<ActionResult<History>> AddToHistory(AddToHistoryRequest addToHistoryRequest)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var historyResult = await _historyService.AddToHistory(userIdResult.Value, addToHistoryRequest.SongFileId);

        if (historyResult.IsFailure)
        {
            return BadRequest(historyResult.Error);
        }

        return Ok(historyResult.Value);
    }
}
