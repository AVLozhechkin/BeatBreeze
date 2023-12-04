using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IHistoryRepository
{
    public Task<History?> GetByUserIdAsync(Guid userId, bool includeHistoryItems, bool asNoTracking);
    public Task AddAsync(History history, bool saveChanges);
}
