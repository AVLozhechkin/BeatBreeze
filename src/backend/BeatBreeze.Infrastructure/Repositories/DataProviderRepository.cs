using BeatBreeze.Core.Enums;
using BeatBreeze.Core.Exceptions;
using BeatBreeze.Core.Interfaces.Repositories;
using BeatBreeze.Core.Models;
using BeatBreeze.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace BeatBreeze.Infrastructure.Repositories;

internal sealed class DataProviderRepository : IDataProviderRepository
{
    private readonly ApplicationContext _applicationContext;

    public DataProviderRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<DataProvider?> GetByIdAsync(Guid providerId, bool includeMusicFiles, bool asNoTracking)
    {
        var query = _applicationContext.DataProviders.AsQueryable();

        if (includeMusicFiles)
        {
            query = query.Include((dp) => dp.MusicFiles);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(dp => dp.Id == providerId);
    }

    public async Task<DataProvider?> GetByTypeAndName(ProviderTypes providerType, string name, Guid userId, bool asNoTracking)
    {
        var query = _applicationContext.DataProviders.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(dp => dp.ProviderType == providerType
                                                     && dp.Name == name
                                                     && dp.UserId == userId);
    }

    public async Task<List<DataProvider>> GetByUserIdAsync(Guid userId, bool includeMusicFiles, bool asNoTracking)
    {
        var query = _applicationContext.DataProviders.AsQueryable();

        if (includeMusicFiles)
        {
            query = query.Include((dp) => dp.MusicFiles);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Where(dp => dp.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(DataProvider dataProvider, bool saveChanges)
    {
        await _applicationContext.DataProviders.AddAsync(dataProvider);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(DataProvider dataProvider, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .DataProviders
                .Where(dp => dp.Id == dataProvider.Id)
                .ExecuteUpdateAsync(setters =>
                    setters
                    .SetProperty(dp => dp.AccessToken, dataProvider.AccessToken)
                    .SetProperty(dp => dp.AccessTokenExpiration, dataProvider.AccessTokenExpiration)
                    .SetProperty(dp => dp.RefreshToken, dataProvider.RefreshToken));

            if (result == 0)
            {
                throw NotFoundException.Create<DataProvider>(dataProvider.Id);
            }

            return;
        }

        _applicationContext.DataProviders.Update(dataProvider);
    }

    public async Task RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .DataProviders
                .Where(dp => dp.Id == dataProviderId && dp.UserId == ownerId)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<DataProvider>(dataProviderId);
            }

            return;
        }

        var provider = new DataProvider() { Id = dataProviderId, UserId = ownerId };
        _applicationContext.DataProviders.Remove(provider);
    }
}
