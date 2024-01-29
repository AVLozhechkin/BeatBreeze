using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class PlaylistRepository : IPlaylistRepository
{
    private readonly ApplicationContext _applicationContext;
    private readonly IExceptionParser _exceptionParser;

    public PlaylistRepository(ApplicationContext applicationContext, IExceptionParser exceptionParser)
    {
        _applicationContext = applicationContext;
        _exceptionParser = exceptionParser;
    }

    public async Task<Playlist?> GetByIdAsync(Guid playlistId, bool includeItems, bool asNoTracking)
    {
        var query = _applicationContext.Playlists.AsQueryable();

        if (includeItems)
        {
            query = query.Include(pl => pl.PlaylistItems)
                .ThenInclude(pi => pi.MusicFile);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(pl => pl.Id == playlistId);
    }

    public async Task<List<Playlist>> GetAllByUserIdAsync(Guid userId, bool includeItems, bool asNoTracking)
    {
        var query = _applicationContext.Playlists.AsQueryable();

        if (includeItems)
        {
            query = query
                .Include(pl => pl.PlaylistItems)
                .ThenInclude(pi => pi.MusicFile);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Where(pl => pl.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(Playlist playlist, bool saveChanges)
    {
        await _applicationContext.Playlists.AddAsync(playlist);

        if (saveChanges)
        {
            try
            {
                await _applicationContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (_exceptionParser.IsAlreadyExists(ex))
            {
                Console.WriteLine(ex.Message);
                throw AlreadyExistException.Create<Playlist>();
            }
        }
    }

    public async Task UpdateAsync(Playlist playlist, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Playlists
                .Where(p => p.Id == playlist.Id)
                .ExecuteUpdateAsync(setters =>
                    setters
                        .SetProperty(p => p.Name, playlist.Name)
                        .SetProperty(p => p.UpdatedAt, playlist.UpdatedAt));

            if (result == 0)
            {
                throw NotFoundException.Create<Playlist>(playlist.Id);
            }

            return;
        }

        _applicationContext.Playlists.Update(playlist);
    }

    public async Task RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Playlists
                .Where(p => p.Id == playlistId && p.UserId == ownerId)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<Playlist>(playlistId);
            }

            return;
        }

        var playlist = new Playlist() { Id = playlistId, UserId = ownerId };
        _applicationContext.Playlists.Remove(playlist);
    }
}
