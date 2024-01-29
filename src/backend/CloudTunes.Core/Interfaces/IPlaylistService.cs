using CloudTunes.Core.Models;

namespace CloudTunes.Core.Interfaces;

public interface IPlaylistService
{
    Task<List<Playlist>> GetPlaylistsByUserId(Guid userId, bool includeItems);
    Task<Playlist> GetPlaylistById(Guid playlistId, Guid userId, bool includeItems);
    Task<IList<PlaylistItem>> GetPlaylistItems(Guid playlistId, Guid userId);
    Task<Playlist> CreatePlaylist(Guid userId, string playlistName);
    Task DeletePlaylist(Guid playlistId, Guid userId);
    Task<Playlist> AddToPlaylist(Guid playlistId, Guid musicFileId, Guid userId);
    Task<Playlist> RemoveFromPlaylist(Guid playlistId, Guid musicFileId, Guid userId);
}
