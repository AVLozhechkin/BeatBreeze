using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IDataProviderRepository
{
    public Task<Result<List<DataProvider>>> GetAllByUserIdAsync(Guid userId, bool includeSongs = false, bool asNoTracking = true);
    public Task<Result<DataProvider?>> GetAsync(Guid providerId, bool includeSongFiles = false, bool asNoTracking = true);
    public Task<Result> AddAsync(DataProvider dataProvider, bool saveChanges = false);
    public Task<Result> UpdateAsync(DataProvider dataProvider, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid dataProviderId, Guid ownerId, bool saveChanges = false);
}
