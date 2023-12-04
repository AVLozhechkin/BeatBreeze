using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.Extensions.Logging;

namespace CloudMusicPlayer.Core.Services;

internal sealed class PlaylistService : IPlaylistService
{
    private readonly ILogger<PlaylistService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PlaylistService(ILogger<PlaylistService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Playlist>> GetPlaylistsByUserId(Guid userId)
    {
        return await _unitOfWork.PlaylistRepository.GetAllByUserIdAsync(userId, true, true);
    }

    public async Task<Playlist> GetPlaylistById(Guid playlistId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, true);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>();
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({userId}) requested a playlist ({playlistId}), " +
                               "but it has another owner ({userId})", userId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>();
        }

        return playlist;
    }

    public async Task<Playlist> CreatePlaylist(Guid userId, string playlistName)
    {
        var playlist = new Playlist(userId, playlistName);

        await _unitOfWork.PlaylistRepository.AddAsync(playlist, true);
        _logger.LogInformation("User ({userId}) created a playlist ({playlistId})",userId, playlist.Id);

        return playlist;
    }

    public async Task DeletePlaylist(Guid playlistId, Guid userId)
    {
        await _unitOfWork.PlaylistRepository.RemoveAsync(playlistId, userId, true);
        _logger.LogInformation("User ({userId}) removed a playlist ({playlistId})", userId, playlistId);
    }

    public async Task<Playlist> AddToPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, true);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({userId}) is trying to add a songFile ({songFileId}) to a " +
                               "playlist ({playlistId}), but it has another owner ({userId})",
                userId, songFileId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>(playlistId);
        }

        var item = new PlaylistItem(playlistId, songFileId);

        await _unitOfWork.PlaylistItemRepository.AddAsync(item, true);

        _logger.LogInformation("User ({userId}) added a songFile ({songFileId}) to a playlist ({playlistId})",
            userId, songFileId, playlistId);

        playlist.PlaylistItems.Add(item);

        return playlist;
    }

    public async Task<Playlist> RemoveFromPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, true);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({userId}) is trying to remove a songFile ({songFileId}) from a " +
                               "playlist ({playlistId}), but it has another owner ({userId})",
                userId, songFileId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>(playlistId);
        }

        var item = playlist.PlaylistItems.FirstOrDefault(i => i.SongFileId == songFileId);

        if (item is null)
        {
            // TODO CHECK WHY I LOOK FOR PLAYLISTITEM BY PLAYLISTID !!!!
            throw NotFoundException.Create<PlaylistItem>(default);
        }

        await _unitOfWork.PlaylistItemRepository.RemoveAsync(item.Id, true);

        _logger.LogInformation("User ({userId}) removed a songFile ({songFileId}) from a playlist ({playlistId})",
            userId, songFileId, playlistId);

        playlist.PlaylistItems.Remove(item);

        return playlist;
    }
}
