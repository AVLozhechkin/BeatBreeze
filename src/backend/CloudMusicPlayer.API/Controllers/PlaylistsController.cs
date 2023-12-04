using Microsoft.AspNetCore.Mvc;
using CloudMusicPlayer.API.Dtos.Models;
using CloudMusicPlayer.API.Dtos.Requests;
using CloudMusicPlayer.API.Filters;
using CloudMusicPlayer.API.Utils;
using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace CloudMusicPlayer.API.Controllers;

[Authorize]
public sealed class PlaylistsController : BaseController
{
    private readonly IPlaylistService _playlistService;

    public PlaylistsController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpPost]
    [ModelValidation]
    public async Task<ActionResult<Playlist>> Create(CreatePlaylistRequest createPlaylistRequest)
    {
        var userId = User.GetUserGuid();

        var playlist = await _playlistService.CreatePlaylist(userId, createPlaylistRequest.Name);

        return Ok(playlist);
    }

    [HttpGet("{playlistId:required}")]
    public async Task<ActionResult<Playlist>> GetById(Guid playlistId)
    {
        var userId = User.GetUserGuid();

        var playlist = await _playlistService.GetPlaylistById(playlistId, userId);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        return Ok(PlaylistDto.Create(playlist));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Playlist>>> GetPlaylistsByUserId()
    {
        var userId = User.GetUserGuid();

        var playlists = await _playlistService.GetPlaylistsByUserId(userId);

        return Ok(playlists.Select(PlaylistDto.Create));
    }

    [HttpDelete("{playlistId:required}")]
    public async Task<IActionResult> Delete(Guid playlistId)
    {
        var userId = User.GetUserGuid();

        await _playlistService.DeletePlaylist(playlistId, userId);

        return Ok();
    }

    [HttpPost("addSong")]
    public async Task<ActionResult<Playlist>> AddSongToPlaylist(PlaylistItemRequest playlistItemRequest)
    {
        var userId = User.GetUserGuid();

        var updatedPlaylist = await _playlistService
            .AddToPlaylist(playlistItemRequest.PlaylistId, playlistItemRequest.SongFileId, userId);

        return Ok(updatedPlaylist);
    }

    [HttpPost("removeSong")]
    public async Task<ActionResult<Playlist>> RemoveSongFromPlaylist(PlaylistItemRequest playlistItemRequest)
    {
        var userId = this.User.GetUserGuid();

        var updatedPlaylist = await _playlistService
            .RemoveFromPlaylist(playlistItemRequest.PlaylistId, playlistItemRequest.SongFileId, userId);

        return Ok(updatedPlaylist);
    }
}
