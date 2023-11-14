using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class DataProviderRepository : IDataProviderRepository
{
    private readonly ApplicationContext _applicationContext;

    public DataProviderRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Result<DataProvider?>> GetAsync(Guid dataProviderId, bool includeSongFiles = false, bool asNoTracking = true)
    {
        var query = _applicationContext.DataProviders.AsQueryable();

        if (includeSongFiles)
        {
            query = query.Include((dp) => dp.SongFiles);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        try
        {
            var provider = await query.FirstOrDefaultAsync(dp => dp.Id == dataProviderId);

            return Result.Success(provider);
        }
        catch (Exception ex)
        {
            return Result.Failure<DataProvider?>(DataLayerErrors.Database.GetError());
        }
    }

    public async Task<Result<List<DataProvider>>> GetAllByUserIdAsync(Guid userId, bool includeSongFiles = false, bool asNoTracking = true)
    {
        var query = _applicationContext.DataProviders.AsQueryable();

        if (includeSongFiles)
        {
            query = query.Include((dp) => dp.SongFiles);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        try
        {
            var providers = await query.Where(dp => dp.UserId == userId)
                .ToListAsync();

            return Result.Success(providers);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<DataProvider>>(DataLayerErrors.Database.GetError());
        }
    }

    public async Task<Result> AddAsync(DataProvider dataProvider, bool saveChanges = false)
    {
        await _applicationContext.DataProviders.AddAsync(dataProvider);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult();
        }

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(DataProvider dataProvider, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeUpdated = _applicationContext.DataProviders.Where(dp => dp.Id == dataProvider.Id);

            return await _applicationContext.ExecuteUpdateResult(toBeUpdated, s =>
                    s.SetProperty(dp => dp.AccessToken, dataProvider.AccessToken)
                        .SetProperty(dp => dp.AccessTokenExpiresAt, dataProvider.AccessTokenExpiresAt)
                        .SetProperty(dp => dp.RefreshToken, dataProvider.RefreshToken));
        }

        _applicationContext.DataProviders.Update(dataProvider);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.DataProviders
                .Where(dp => dp.Id == dataProviderId && dp.UserId == ownerId).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted);
        }

        try
        {
            var provider =
                await _applicationContext.DataProviders.FirstOrDefaultAsync(dp =>
                    dp.Id == dataProviderId && dp.UserId == ownerId);

            if (provider is null)
            {
                return Result.Failure(DataLayerErrors.Database.NotFound());
            }

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(DataLayerErrors.Database.DeleteError());
        }
    }
}
