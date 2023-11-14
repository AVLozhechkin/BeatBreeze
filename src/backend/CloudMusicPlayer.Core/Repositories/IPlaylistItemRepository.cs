using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IPlaylistItemRepository
{
    public Task<Result> AddAsync(PlaylistItem playlistItem, bool saveChanges = false);

    public Task<Result> RemoveAsync(Guid playlistItemId, bool saveChanges = false);
}
