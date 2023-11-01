using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IPlaylistRepository
{
    public Task<Playlist?> GetByIdAsync(Guid playlistId, bool asNoTracking = true);
    public Task<List<Playlist>> GetAllByUserIdAsync(Guid userId, bool asNoTracking = true);
    public Task<Result> AddAsync(Playlist playlist, bool saveChanges = false);
    public Task<Result> UpdateAsync(Playlist playlist, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges = false);
}
