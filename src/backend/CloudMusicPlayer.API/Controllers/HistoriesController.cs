using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudMusicPlayer.API.Controllers;

[Authorize]
public sealed class HistoriesController : BaseController
{
    private readonly ILogger<HistoriesController> _logger;
    private readonly IHistoryService _historyService;

    public HistoriesController(ILogger<HistoriesController> logger, IHistoryService historyService)
    {
        _logger = logger;
        _historyService = historyService;
    }

    [HttpGet]
    public async Task<ActionResult<History?>> GetUserHistory()
    {
        var userId = this.User.GetUserGuid();

        var history = await _historyService.GetUserHistory(userId);

        return Ok(history);
    }

    [HttpPost("add")]
    public async Task<ActionResult<History>> AddToHistory(AddToHistoryRequest addToHistoryRequest)
    {
        var userId = this.User.GetUserGuid();

        var history = await _historyService.AddToHistory(userId, addToHistoryRequest.SongFileId);

        return Ok(history);
    }
}
