using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;
public sealed class DataProviderRepository : IDataProviderRepository
{
    private readonly ApplicationContext _applicationContext;

    public DataProviderRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<DataProvider?> GetAsync(Guid dataProviderId, bool includeSongFiles = false, bool asNoTracking = true)
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

        var provider = await query.FirstOrDefaultAsync(dp => dp.Id == dataProviderId);

        return provider;
    }


    public async Task<List<DataProvider>> GetAllByUserIdAsync(Guid userId, bool includeSongFiles = false, bool asNoTracking = true)
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

        return await query.Where(dp => dp.UserId == userId)
            .ToListAsync();
    }

    public async Task<Result> AddAsync(DataProvider dataProvider, bool saveChanges = false)
    {
        await _applicationContext.DataProviders.AddAsync(dataProvider);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult("Data provider was not added");
        }

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(DataProvider dataProvider, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeUpdated = _applicationContext.DataProviders.Where(dp => dp.Id == dataProvider.Id);

            return await _applicationContext.ExecuteUpdateResult(toBeUpdated, s =>
                    s.SetProperty(dp => dp.AccessToken.Token, dataProvider.AccessToken.Token)
                        .SetProperty(dp => dp.RefreshToken, dataProvider.RefreshToken)
                        .SetProperty(dp => dp.AccessToken.ExpiresAt, dataProvider.AccessToken.ExpiresAt),
                "Data provider was not updated");
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

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted, "Data provider was not deleted");
        }

        var provider = await _applicationContext.DataProviders.FirstOrDefaultAsync(dp => dp.Id == dataProviderId && dp.UserId == ownerId);

        if (provider is null)
        {
            return Result.Failure("Data provider was not found");
        }

        _applicationContext.DataProviders.Remove(provider);

        return Result.Success();
    }
}
