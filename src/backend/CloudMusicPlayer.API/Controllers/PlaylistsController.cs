using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using Microsoft.AspNetCore.Mvc;
using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Utils;

namespace CloudMusicPlayer.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlaylistsController : ControllerBase
{
    private readonly PlaylistService _playlistService;

    public PlaylistsController(PlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpPost]
    public async Task<ActionResult<Playlist>> Create(CreatePlaylistRequest createPlaylistRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var result = await _playlistService.CreatePlaylist(userIdResult.Value, createPlaylistRequest.Name);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpGet("{playlistId:required}")]
    public async Task<ActionResult<Playlist>> GetById(Guid playlistId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var playlistResult = await _playlistService.GetPlaylistById(playlistId, userIdResult.Value);

        if (playlistResult.Value is null)
        {
            return NotFound("Playlist was not found");
        }

        if (playlistResult.IsFailure)
        {
            return BadRequest(playlistResult.Error);
        }

        return Ok(PlaylistDto.Create(playlistResult.Value));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylistsByUserId()
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var result = await _playlistService.GetPlaylistsByUserId(userIdResult.Value);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value.Select(PlaylistDto.Create));
    }

    [HttpDelete("{playlistId:required}")]
    public async Task<IActionResult> Delete(Guid playlistId)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var result = await _playlistService.DeletePlaylist(playlistId, userIdResult.Value);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok();
    }

    [HttpPost("addSong")]
    public async Task<ActionResult<Playlist>> AddSongToPlaylist(PlaylistItemRequest playlistItemRequest)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var result = await this._playlistService.AddToPlaylist(playlistItemRequest.PlaylistId,
            playlistItemRequest.SongFileId, userIdResult.Value);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }

    [HttpPost("removeSong")]
    public async Task<ActionResult<Playlist>> RemoveSongFromPlaylist(PlaylistItemRequest playlistItemRequest)
    {
        var userIdResult = this.User.GetUserGuid();

        if (userIdResult.IsFailure)
        {
            return Unauthorized("Something wrong with cookies. Please re-login.");
        }

        var result = await this._playlistService.RemoveFromPlaylist(playlistItemRequest.PlaylistId,
            playlistItemRequest.SongFileId, userIdResult.Value);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);
    }
}
