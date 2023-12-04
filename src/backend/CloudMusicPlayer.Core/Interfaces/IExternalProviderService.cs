using CloudMusicPlayer.Core.Enums;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<IReadOnlyList<SongFile>> GetSongFiles(DataProvider provider);
    public Task<string> GetSongFileUrl(SongFile songFile, DataProvider provider);
    public Task<AccessToken> GetAccessToken(byte[] refreshToken);
}
