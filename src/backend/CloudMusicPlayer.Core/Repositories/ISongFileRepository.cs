using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface ISongFileRepository
{
    public Task<SongFile?> GetById(Guid songFileId, bool includeDataProvider, bool asNoTracking = true);
    public Task<Result> AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges = false);
    public Task<Result> RemoveAsync(SongFile songFile, bool saveChanges = false);
}
