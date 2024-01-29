using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IMusicFileRepository
{
    public Task<MusicFile?> GetById(Guid musicFileId, bool includeDataProvider, bool asNoTracking);
    public Task AddRangeAsync(IEnumerable<MusicFile> musicFiles, bool saveChanges);
    public Task RemoveAsync(MusicFile musicFile, bool saveChanges);
}
