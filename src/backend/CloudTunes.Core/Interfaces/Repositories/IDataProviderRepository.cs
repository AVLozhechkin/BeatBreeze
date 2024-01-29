using CloudTunes.Core.Enums;
using CloudTunes.Core.Models;

namespace CloudTunes.Core.Interfaces.Repositories;

public interface IDataProviderRepository
{
    public Task<List<DataProvider>> GetByUserIdAsync(Guid userId, bool includeMusicFiles, bool asNoTracking);
    public Task<DataProvider?> GetByIdAsync(Guid providerId, bool includeMusicFiles, bool asNoTracking);

    public Task<DataProvider?>
        GetByTypeAndName(ProviderTypes providerType, string name, Guid userId, bool asNoTracking);
    public Task AddAsync(DataProvider dataProvider, bool saveChanges);
    public Task UpdateAsync(DataProvider dataProvider, bool saveChanges);
    public Task RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges);
}
