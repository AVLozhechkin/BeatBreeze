using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IHistoryRepository
{
    public Task<Result> AddAsync(History history, bool saveChanges = false);
    public Task<Result<History?>> GetByUserIdAsync(Guid userId, bool includeHistoryItems = false, bool asNoTracking = true);
}
