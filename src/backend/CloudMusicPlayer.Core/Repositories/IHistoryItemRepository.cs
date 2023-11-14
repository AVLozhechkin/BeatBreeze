using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IHistoryItemRepository
{
    public Task<Result> AddAsync(HistoryItem historyItem, bool saveChanges = false);
}
