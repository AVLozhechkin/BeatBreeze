using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<IReadOnlyList<MusicFile>> GetMusicFiles(DataProvider provider);
    public Task<string> GetMusicFileUrl(MusicFile musicFile, DataProvider provider);
    public Task<AccessToken> GetAccessToken(DataProvider provider);
}
