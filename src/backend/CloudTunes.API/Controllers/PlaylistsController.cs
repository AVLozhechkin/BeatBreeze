using Microsoft.AspNetCore.Mvc;
using CloudTunes.API.Dtos.Requests;
using CloudTunes.API.Filters;
using CloudTunes.API.Utils;
using CloudTunes.Core.Exceptions;
using CloudTunes.Core.Interfaces;
using CloudTunes.Core.Models;
using Microsoft.AspNetCore.Authorization;
using CloudTunes.API.Dtos.Models;

namespace CloudTunes.API.Controllers;

[Authorize]
public sealed class PlaylistsController : BaseController
{
    private readonly IPlaylistService _playlistService;

    public PlaylistsController(IPlaylistService playlistService)
    {
        _playlistService = playlistService;
    }

    [HttpGet]
    public async Task<IEnumerable<PlaylistDto>> GetPlaylistsByUserId(bool includeItems = false)
    {
        var userId = User.GetUserGuid();

        var playlists = await _playlistService.GetPlaylistsByUserId(userId, includeItems);

        return playlists.Select(PlaylistDto.Create);
    }

    [HttpGet("{playlistId:required}")]
    public async Task<PlaylistDto> GetById(Guid playlistId, bool includeItems)
    {
        var userId = User.GetUserGuid();

        var playlist = await _playlistService.GetPlaylistById(playlistId, userId, includeItems);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        return PlaylistDto.Create(playlist);
    }

    [HttpPost]
    [ModelValidation]
    public async Task<PlaylistDto> Create(CreatePlaylistRequest createPlaylistRequest)
    {
        ArgumentNullException.ThrowIfNull(createPlaylistRequest);

        var userId = User.GetUserGuid();

        var playlist = await _playlistService.CreatePlaylist(userId, createPlaylistRequest.Name);

        return PlaylistDto.Create(playlist);
    }


    [HttpDelete("{playlistId:required}")]
    public async Task Delete(Guid playlistId)
    {
        var userId = User.GetUserGuid();

        await _playlistService.DeletePlaylist(playlistId, userId);
    }

    [HttpPost("{playlistId}/add")]
    public async Task<PlaylistDto> AddSongToPlaylist(PlaylistItemRequest playlistItemRequest, Guid playlistId)
    {
        ArgumentNullException.ThrowIfNull(playlistItemRequest);

        var userId = User.GetUserGuid();

        var updatedPlaylist = await _playlistService
            .AddToPlaylist(playlistId, playlistItemRequest.MusicFileId, userId);

        return PlaylistDto.Create(updatedPlaylist);
    }

    [HttpPost("{playlistId}/remove")]
    public async Task<PlaylistDto> RemoveSongFromPlaylist(PlaylistItemRequest playlistItemRequest, Guid playlistId)
    {
        ArgumentNullException.ThrowIfNull(playlistItemRequest);

        var userId = User.GetUserGuid();

        var updatedPlaylist = await _playlistService
            .RemoveFromPlaylist(playlistId, playlistItemRequest.MusicFileId, userId);

        return PlaylistDto.Create(updatedPlaylist);
    }
}
