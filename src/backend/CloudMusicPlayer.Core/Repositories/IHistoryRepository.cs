using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IHistoryRepository
{
    public Task<Result> AddAsync(History history, bool saveChanges = false);
    public Task<History?> GetByUserIdAsync(Guid userId, bool includeHistoryItems = false, bool asNoTracking = true);
}
