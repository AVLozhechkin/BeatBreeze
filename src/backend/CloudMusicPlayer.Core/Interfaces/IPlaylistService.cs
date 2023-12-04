using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IPlaylistService
{
    Task<List<Playlist>> GetPlaylistsByUserId(Guid userId);
    Task<Playlist> GetPlaylistById(Guid playlistId, Guid userId);
    Task<Playlist> CreatePlaylist(Guid userId, string playlistName);
    Task DeletePlaylist(Guid playlistId, Guid userId);
    Task<Playlist> AddToPlaylist(Guid playlistId, Guid songFileId, Guid userId);
    Task<Playlist> RemoveFromPlaylist(Guid playlistId, Guid songFileId, Guid userId);
}
