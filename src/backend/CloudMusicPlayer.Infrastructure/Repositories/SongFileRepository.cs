using System.Data;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Infrastructure.Repositories;
internal class SongFileRepository : ISongFileRepository
{
    private readonly ApplicationContext _applicationContext;

    public SongFileRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Result> AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges = false)
    {
        await _applicationContext.SongFiles.AddRangeAsync(songFiles);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult();
        }

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(SongFile songFile, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.SongFiles
                .Where(p => p.Id == songFile.Id).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted, "Playlist was not deleted");
        }

        _applicationContext.SongFiles.Remove(songFile);

        return Result.Success();
    }
}
