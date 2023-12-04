using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IProviderService
{
    Task<List<DataProvider>> GetAllProvidersByUserId(Guid userId);
    Task<DataProvider> GetDataProvider(Guid providerId, Guid userId);
    Task<string> GetSongFileUrl(Guid songFileId, Guid userId);

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
