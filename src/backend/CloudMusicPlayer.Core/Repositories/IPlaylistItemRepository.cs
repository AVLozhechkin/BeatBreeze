using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IPlaylistItemRepository
{
    public Task<Result> AddAsync(PlaylistItem playlistItem, bool saveChanges = false);

    public Task<Result> RemoveAsync(Guid playlistItemId, bool saveChanges = false);
}
