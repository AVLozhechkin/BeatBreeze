using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IPlaylistRepository
{
    public Task<Result<Playlist?>> GetByIdAsync(Guid playlistId, bool includeSongFiles, bool asNoTracking = true);
    public Task<Result<List<Playlist>>> GetAllByUserIdAsync(Guid userId, bool includeSongFiles, bool asNoTracking = true);
    public Task<Result> AddAsync(Playlist playlist, bool saveChanges = false);
    public Task<Result> UpdateAsync(Playlist playlist, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges = false);
}
