using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;

namespace CloudMusicPlayer.Core.Services;

public sealed class PlaylistService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlaylistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<Playlist>>> GetPlaylistsByUserId(Guid userId)
    {
        var playlistsResult = await _unitOfWork.PlaylistRepository.GetAllByUserIdAsync(userId, true);

        return playlistsResult;
    }

    public async Task<Result<Playlist?>> GetPlaylistById(Guid playlistId, Guid userId)
    {
        var playlistResult = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true);

        if (playlistResult.IsFailure)
        {
            return playlistResult;
        }

        if (playlistResult.Value is not null && playlistResult.Value.UserId != userId)
        {
            return Result.Failure<Playlist?>(DomainLayerErrors.NotTheOwner());
        }

        return playlistResult;
    }

    public async Task<Result<Playlist>> CreatePlaylist(Guid userId, string playlistName)
    {
        var playlistCreationResult = Playlist.Create(userId, playlistName);

        var addResult = await _unitOfWork.PlaylistRepository.AddAsync(playlistCreationResult.Value, true);

        if (addResult.IsFailure)
        {
            return Result.Failure<Playlist>(addResult.Error);
        }

        return playlistCreationResult;
    }

    public async Task<Result> DeletePlaylist(Guid playlistId, Guid userId)
    {
        var removeResult = await _unitOfWork.PlaylistRepository.RemoveAsync(playlistId, userId, true);

        if (removeResult.IsFailure)
        {
            return Result.Failure(removeResult.Error);
        }

        return removeResult;
    }

    public async Task<Result<Playlist>> AddToPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlistResult = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, false);

        if (playlistResult.IsFailure)
        {
            return playlistResult;
        }

        if (playlistResult.Value is null)
        {
            return Result.Failure<Playlist>(DomainLayerErrors.NotFound());
        }

        if (playlistResult.Value.UserId != userId)
        {
            return Result.Failure<Playlist>(DomainLayerErrors.NotTheOwner());
        }

        var item = new PlaylistItem
        {
            PlaylistId = playlistId,
            SongFileId = songFileId
        };

        var addResult = await _unitOfWork.PlaylistItemRepository.AddAsync(item, true);

        if (addResult.IsFailure)
        {
            return Result.Failure<Playlist>(addResult.Error);
        }

        playlistResult.Value.PlaylistItems.Add(item);

        return playlistResult;
    }

    public async Task<Result<Playlist>> RemoveFromPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlistResult = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId, true);

        if (playlistResult.IsFailure)
        {
            return playlistResult;
        }

        if (playlistResult.Value is null)
        {
            return Result.Failure<Playlist>(DomainLayerErrors.NotFound());
        }

        if (playlistResult.Value.UserId != userId)
        {
            return Result.Failure<Playlist>(DomainLayerErrors.NotTheOwner());
        }


        var item = playlistResult.Value.PlaylistItems.FirstOrDefault(i => i.SongFileId == songFileId);

        if (item is null)
        {
            return Result.Failure<Playlist>(DomainLayerErrors.Playlist.NoPlaylistItem());
        }

        var result = await _unitOfWork.PlaylistItemRepository.RemoveAsync(item.Id, true);

        if (result.IsFailure)
        {
            return Result.Failure<Playlist>(result.Error);
        }

        playlistResult.Value.PlaylistItems.Remove(item);

        return playlistResult;
    }
}
