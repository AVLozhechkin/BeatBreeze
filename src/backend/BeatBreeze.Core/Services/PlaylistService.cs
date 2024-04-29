using BeatBreeze.Core.Exceptions;
using BeatBreeze.Core.Interfaces;
using BeatBreeze.Core.Models;
using Microsoft.Extensions.Logging;

namespace BeatBreeze.Core.Services;

internal sealed class PlaylistService : IPlaylistService
{
    private readonly ILogger<PlaylistService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PlaylistService(ILogger<PlaylistService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Playlist>> GetPlaylistsByUserId(Guid userId, bool includeItems)
    {
        return await _unitOfWork.PlaylistRepository.GetAllByUserIdAsync(userId, includeItems, true);
    }

    public async Task<Playlist> GetPlaylistById(Guid playlistId, Guid userId, bool includeItems)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, includeItems, true);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>();
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) requested a playlist ({PlaylistId}), " +
                               "but it has another owner ({UserId})", userId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>();
        }

        return playlist;
    }

    public async Task<IList<PlaylistItem>> GetPlaylistItems(Guid playlistId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, true);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>();
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) requested a playlist items for a playlist ({PlaylistId}), " +
                               "but it has another owner ({UserId})", userId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>();
        }

        return playlist.PlaylistItems;
    }

    public async Task<Playlist> CreatePlaylist(Guid userId, string playlistName)
    {
        var playlist = new Playlist(userId, playlistName);

        await _unitOfWork.PlaylistRepository.AddAsync(playlist, true);
        _logger.LogInformation("User ({UserId}) created a playlist ({PlaylistId})", userId, playlist.Id);

        return playlist;
    }

    public async Task DeletePlaylist(Guid playlistId, Guid userId)
    {
        await _unitOfWork.PlaylistRepository.RemoveAsync(playlistId, userId, true);
        _logger.LogInformation("User ({UserId}) removed a playlist ({PlaylistId})", userId, playlistId);
    }

    public async Task<Playlist> AddToPlaylist(Guid playlistId, Guid musicFileId, Guid userId)
    {
        // First we need to get playlist and check if person is an owner of it
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, false);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) is trying to add a musicFile ({MusicFileId}) to a " +
                               "playlist ({PlaylistId}), but it has another owner ({UserId})",
                userId, musicFileId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>(playlistId);
        }

        // Second we need to check if item is already in a playlist
        if (playlist.PlaylistItems.FirstOrDefault(pi => pi.MusicFileId == musicFileId) is not null)
        {
            throw AlreadyExistException.Create<PlaylistItem>();
        }

        // Third we need to get musicFile and check if person is an owner of it
        var musicFile = await _unitOfWork.MusicFileRepository.GetById(musicFileId, true, true);

        if (musicFile is null)
        {
            throw NotFoundException.Create<MusicFile>(musicFileId);
        }

        if (musicFile.DataProvider.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) is trying to add a musicFile ({MusicFileId}) to a " +
                               "playlist ({PlaylistId}), but musicFile has another owner ({UserId})",
                userId, musicFileId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<MusicFile>(musicFileId);
        }

        var item = new PlaylistItem(playlistId, musicFileId);

        await _unitOfWork.PlaylistItemRepository.AddAsync(item, false);

        // TODO Fix playlist update, probably it should be placed in playlist but it requires rich domain model mehhhh
        playlist.UpdatedAt = DateTimeOffset.UtcNow;
        playlist.Size++;

        await _unitOfWork.CommitAsync();

        item.MusicFile = musicFile;

        _logger.LogInformation("User ({UserId}) added a musicFile ({MusicFileId}) to a playlist ({PlaylistId})",
            userId, musicFileId, playlistId);

        return playlist;
    }

    public async Task<Playlist> RemoveFromPlaylist(Guid playlistId, Guid musicFileId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true, false);

        if (playlist is null)
        {
            throw NotFoundException.Create<Playlist>(playlistId);
        }

        if (playlist.UserId != userId)
        {
            _logger.LogWarning("User ({UserId}) is trying to remove a musicFile ({MusicFileId}) from a " +
                               "playlist ({PlaylistId}), but it has another owner ({UserId})",
                userId, musicFileId, playlistId, playlist.UserId);
            throw NotTheOwnerException.Create<Playlist>(playlistId);
        }

        var item = playlist.PlaylistItems.FirstOrDefault(i => i.MusicFileId == musicFileId);

        if (item is null)
        {
            throw NotFoundException.Create<PlaylistItem>(Guid.Empty);
        }

        await _unitOfWork.PlaylistItemRepository.RemoveAsync(item, false);

        playlist.UpdatedAt = DateTimeOffset.UtcNow;
        playlist.Size = playlist.Size - 1;

        await _unitOfWork.CommitAsync();

        _logger.LogInformation("User ({UserId}) removed a musicFile ({MusicFileId}) from a playlist ({PlaylistId})",
            userId, musicFileId, playlistId);

        return playlist;
    }
}
