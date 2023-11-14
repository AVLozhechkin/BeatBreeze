using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface ISongFileRepository
{
    public Task<Result<SongFile?>> GetById(Guid songFileId, bool includeDataProvider, bool asNoTracking = true);
    public Task<Result> AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges = false);
    public Task<Result> RemoveAsync(SongFile songFile, bool saveChanges = false);
}
