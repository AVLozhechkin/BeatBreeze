using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class HistoryItemRepository : IHistoryItemRepository
{
    private readonly ApplicationContext _applicationContext;

    public HistoryItemRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }
    public async Task AddAsync(HistoryItem historyItem, bool saveChanges = false)
    {
        await _applicationContext.HistoryItems.AddAsync(historyItem);
    }
}
