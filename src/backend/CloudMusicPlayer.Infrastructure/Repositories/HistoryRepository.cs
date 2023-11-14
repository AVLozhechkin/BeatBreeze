using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class HistoryRepository : IHistoryRepository
{
    private readonly ApplicationContext _applicationContext;

    public HistoryRepository(ApplicationContext applicationContext)
    {
        this._applicationContext = applicationContext;
    }

    public async Task<Result> AddAsync(History history, bool saveChanges = false)
    {
        await _applicationContext.Histories.AddAsync(history);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult();
        }

        return Result.Success();
    }

    public async Task<Result<History?>> GetByUserIdAsync(Guid userId, bool includeHistoryItems = false, bool asNoTracking = true)
    {
        var query = _applicationContext.Histories.AsQueryable();

        if (includeHistoryItems)
        {
            query = query.Include(h => h.HistoryItems);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        try
        {
            var provider = await query.FirstOrDefaultAsync(h => h.UserId == userId);

            return Result.Success(provider);
        }
        catch (Exception ex)
        {
            return Result.Failure<History?>(DataLayerErrors.Database.GetError());
        }
    }
}
