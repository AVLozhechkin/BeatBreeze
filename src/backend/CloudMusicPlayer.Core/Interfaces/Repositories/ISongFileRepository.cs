using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface ISongFileRepository
{
    public Task<SongFile?> GetById(Guid songFileId, bool includeDataProvider, bool asNoTracking);
    public Task AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges);
    public Task RemoveAsync(SongFile songFile, bool saveChanges);
}
