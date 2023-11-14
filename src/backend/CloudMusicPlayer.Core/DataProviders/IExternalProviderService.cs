using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.DataProviders;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<Result<IReadOnlyList<SongFile>>> GetSongFiles(DataProvider provider);
    public Task<Result<string>> GetSongFileUrl(SongFile songFile, DataProvider provider);
    public Task<Result<AccessToken>> GetApiToken(string refreshToken);
}
