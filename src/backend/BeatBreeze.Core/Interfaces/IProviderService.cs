using BeatBreeze.Core.Enums;
using BeatBreeze.Core.Models;

namespace BeatBreeze.Core.Interfaces;

public interface IProviderService
{
    Task<List<DataProvider>> GetProvidersByUserId(Guid userId, bool includeFiles);
    Task<DataProvider> GetDataProviderById(Guid providerId, Guid userId, bool includeFiles);
    Task<IEnumerable<MusicFile>> GetSongsForDataProvider(Guid providerId, Guid userId);
    Task<string> GetMusicFileUrl(Guid musicFileId, Guid userId);

    Task AddDataProvider(
        ProviderTypes providerType,
        Guid userId,
        string name,
        string accessToken,
        string refreshToken,
        string expiresAt);

    Task<DataProvider> UpdateDataProvider(Guid providerId, Guid userId);
    Task RemoveDataProvider(Guid providerId, Guid userId);
}
