using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;
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

    public async Task AddRangeAsync(IEnumerable<SongFile> songFiles, bool saveChanges = false)
    {
        await _applicationContext.SongFiles.AddRangeAsync(songFiles);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAsync(SongFile songFile, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .SongFiles
                .Where(p => p.Id == songFile.Id)
                .Take(1)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<SongFile>(songFile.Id);
            }

            return;
        }

        _applicationContext.SongFiles.Remove(songFile);
    }
}
