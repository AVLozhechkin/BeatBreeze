using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Services;

public class PlaylistService
{
    private readonly IUnitOfWork _unitOfWork;

    public PlaylistService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<IEnumerable<Playlist>>> GetPlaylistsByUserId(Guid userId)
    {
        var playlists = await _unitOfWork.PlaylistRepository.GetAllByUserIdAsync(userId);

        return Result.Success<IEnumerable<Playlist>>(playlists);
    }

    public async Task<Result<Playlist?>> GetPlaylistById(Guid playlistId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId);

        if (playlist is null)
        {
            return Result.Success(playlist);
        }

        if (playlist.UserId != userId)
        {
            return Result.Failure<Playlist?>("User is not the owner of the Playlist");
        }

        return Result.Success<Playlist?>(playlist);
    }

    public async Task<Result<Playlist>> CreatePlaylist(Guid userId, string playlistName)
    {
        var playlistCreationResult = Playlist.Create(userId, playlistName);

        var result = await _unitOfWork.PlaylistRepository.AddAsync(playlistCreationResult.Value, true);

        if (result.IsFailure)
        {
            return Result.Failure<Playlist>(result.Error);
        }

        return Result.Success(playlistCreationResult.Value);
    }

    public async Task<Result> DeletePlaylist(Guid playlistId, Guid userId)
    {
        var result = await _unitOfWork.PlaylistRepository.RemoveAsync(playlistId, userId, true);

        if (result.IsFailure)
        {
            return Result.Failure<Playlist>(result.Error);
        }

        return Result.Success();
    }

    public async Task<Result<Playlist>> AddToPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId);

        if (playlist is null)
        {
            return Result.Failure<Playlist>("Playlist was not found");
        }

        if (playlist.UserId != userId)
        {
            return Result.Failure<Playlist>("User is not the owner of the Playlist");
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

        playlist.PlaylistItems.Add(item);

        return Result.Success(playlist);
    }

    public async Task<Result<Playlist>> RemoveFromPlaylist(Guid playlistId, Guid songFileId, Guid userId)
    {
        var playlist = await _unitOfWork.PlaylistRepository.GetByIdAsync(playlistId);

        if (playlist is null)
        {
            return Result.Failure<Playlist>("Playlist was not found");
        }

        if (playlist.UserId != userId)
        {
            return Result.Failure<Playlist>("User is not the owner of the Playlist");
        }


        var item = playlist.PlaylistItems.FirstOrDefault(i => i.SongFileId == songFileId);

        if (item is null)
        {
            return Result.Failure<Playlist>("There is no such element in playlist");
        }

        var result = await _unitOfWork.PlaylistItemRepository.RemoveAsync(item.Id, true);

        if (result.IsFailure)
        {
            return Result.Failure<Playlist>(result.Error);
        }

        playlist.PlaylistItems.Remove(item);

        return Result.Success(playlist);
    }
}
