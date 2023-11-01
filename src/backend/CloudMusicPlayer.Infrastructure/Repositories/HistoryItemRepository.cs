using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Infrastructure.Repositories;

public class HistoryItemRepository : IHistoryItemRepository
{
    private readonly ApplicationContext _applicationContext;

    public HistoryItemRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    public async Task<Result> AddAsync(HistoryItem historyItem, bool saveChanges = false)
    {
        await _applicationContext.HistoryItems.AddAsync(historyItem);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult("History Item was not added");
        }

        return Result.Success();
    }
}
