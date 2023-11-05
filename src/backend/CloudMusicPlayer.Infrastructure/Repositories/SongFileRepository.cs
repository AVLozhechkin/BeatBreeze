using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class SongFileRepository : ISongFileRepository
{
    private readonly ApplicationContext _applicationContext;

    public SongFileRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<SongFile?> GetById(Guid songFileId, bool includeDataProvider, bool asNoTracking = true)
    {
        var query = _applicationContext.SongFiles.AsQueryable();

        if (includeDataProvider)
        {
            query = query.Include(sf => sf.DataProvider);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(pl => pl.Id == songFileId);
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
