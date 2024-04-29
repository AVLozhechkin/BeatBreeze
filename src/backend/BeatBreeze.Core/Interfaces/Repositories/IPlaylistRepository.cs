using BeatBreeze.Core.Models;

namespace BeatBreeze.Core.Interfaces.Repositories;

public interface IPlaylistRepository
{
    public Task<Playlist?> GetByIdAsync(Guid playlistId, bool includeItems, bool asNoTracking);
    public Task<List<Playlist>> GetAllByUserIdAsync(Guid userId, bool includeItems, bool asNoTracking);
    public Task AddAsync(Playlist playlist, bool saveChanges);
    public Task UpdateAsync(Playlist playlist, bool saveChanges);
    public Task RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges);
}
