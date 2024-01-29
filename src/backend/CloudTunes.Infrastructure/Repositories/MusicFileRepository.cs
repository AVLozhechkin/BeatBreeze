using CloudTunes.Core.Exceptions;
using CloudTunes.Core.Interfaces.Repositories;
using CloudTunes.Core.Models;
using CloudTunes.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudTunes.Infrastructure.Repositories;

internal sealed class MusicFileRepository : IMusicFileRepository
{
    private readonly ApplicationContext _applicationContext;

    public MusicFileRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<MusicFile?> GetById(Guid musicFileId, bool includeDataProvider, bool asNoTracking)
    {
        var query = _applicationContext.MusicFiles.AsQueryable();

        if (includeDataProvider)
        {
            query = query.Include(sf => sf.DataProvider);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(pl => pl.Id == musicFileId);
    }

    public async Task AddRangeAsync(IEnumerable<MusicFile> musicFiles, bool saveChanges)
    {
        await _applicationContext.MusicFiles.AddRangeAsync(musicFiles);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAsync(MusicFile musicFile, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .MusicFiles
                .Where(p => p.Id == musicFile.Id)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<MusicFile>(musicFile.Id);
            }

            return;
        }

        _applicationContext.MusicFiles.Remove(musicFile);
    }
}
