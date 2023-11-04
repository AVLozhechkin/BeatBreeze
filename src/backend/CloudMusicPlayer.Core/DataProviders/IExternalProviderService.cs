using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.DataProviders;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<IReadOnlyList<SongFile>> GetSongFiles(DataProvider provider);
    public Task<string?> GetSongFileUrl(SongFile songFile, DataProvider provider);
    public Task<AccessTokenResult?> GetApiToken(string refreshToken);
}
