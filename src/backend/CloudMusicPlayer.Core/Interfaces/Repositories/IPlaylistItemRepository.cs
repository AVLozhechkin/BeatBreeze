using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IPlaylistItemRepository
{
    public Task AddAsync(PlaylistItem playlistItem, bool saveChanges);

    public Task RemoveAsync(PlaylistItem playlistItem, bool saveChanges);
}
