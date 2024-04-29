using BeatBreeze.Core.Enums;
using BeatBreeze.Core.Models;

namespace BeatBreeze.Core.Interfaces;

public interface IExternalProviderService
{
    public bool CanBeExecuted(ProviderTypes providerType);
    public Task<IReadOnlyList<MusicFile>> GetMusicFiles(DataProvider provider);
    public Task<string> GetMusicFileUrl(MusicFile musicFile, DataProvider provider);
    public Task<AccessToken> GetAccessToken(DataProvider provider);
}
