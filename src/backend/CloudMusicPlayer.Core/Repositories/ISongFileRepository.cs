using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface ISongFileRepository
{
    public Task<Result> AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges = false);
    public Task<Result> RemoveAsync(SongFile songFile, bool saveChanges = false);
}
