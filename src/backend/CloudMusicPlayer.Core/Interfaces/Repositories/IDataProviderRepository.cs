using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IDataProviderRepository
{
    public Task<List<DataProvider>> GetAllByUserIdAsync(Guid userId, bool includeSongs, bool asNoTracking);
    public Task<DataProvider?> GetByIdAsync(Guid providerId, bool includeSongFiles, bool asNoTracking);

    public Task<DataProvider?>
        GetByTypeAndName(ProviderTypes providerType, string name, Guid userId, bool asNoTracking);
    public Task AddAsync(DataProvider dataProvider, bool saveChanges);
    public Task UpdateAsync(DataProvider dataProvider, bool saveChanges);
    public Task RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges);
}
