using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IHistoryItemRepository
{
    public Task<Result> AddAsync(HistoryItem historyItem, bool saveChanges = false);
}
