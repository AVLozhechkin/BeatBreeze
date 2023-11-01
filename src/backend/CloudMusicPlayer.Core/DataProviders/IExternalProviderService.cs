using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.DataProviders;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<SongFile[]> GetSongFiles(DataProvider provider);
}
