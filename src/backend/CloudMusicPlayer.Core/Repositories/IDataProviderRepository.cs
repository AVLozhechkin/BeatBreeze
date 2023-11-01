using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IDataProviderRepository
{
    public Task<List<DataProvider>> GetAllByUserIdAsync(Guid userId, bool includeSongs = false, bool asNoTracking = true);
    public Task<DataProvider?> GetAsync(Guid providerId, bool includeSongFiles = false, bool asNoTracking = true);
    public Task<Result> AddAsync(DataProvider dataProvider, bool saveChanges = false);
    public Task<Result> UpdateAsync(DataProvider dataProvider, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges = false);
}
