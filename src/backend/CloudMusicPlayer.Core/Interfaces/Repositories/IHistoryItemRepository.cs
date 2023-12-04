using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IHistoryItemRepository
{
    public Task AddAsync(HistoryItem historyItem, bool saveChanges);
}
