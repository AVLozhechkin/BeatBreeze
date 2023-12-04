using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class PlaylistRepository : IPlaylistRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Playlist?> GetByIdAsync(Guid playlistId, bool includeSongFiles, bool asNoTracking = true)
    {
        var query = _applicationContext.Playlists.AsQueryable();

        if (includeSongFiles)
        {
            query = query.Include(pl => pl.PlaylistItems)
                .ThenInclude(pi => pi.SongFile);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(pl => pl.Id == playlistId);
    }

    public async Task<List<Playlist>> GetAllByUserIdAsync(Guid userId, bool includeSongFiles, bool asNoTracking = true)
    {
        var query = _applicationContext.Playlists.AsQueryable();

        if (includeSongFiles)
        {
            query = query
                .Include(pl => pl.PlaylistItems)
                .ThenInclude(pi => pi.SongFile);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Where(pl => pl.UserId == userId).ToListAsync();
    }

    public async Task AddAsync(Playlist playlist, bool saveChanges = false)
    {
        await _applicationContext.Playlists.AddAsync(playlist);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(Playlist playlist, bool saveChanges = false)
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

    public async Task RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Playlists
                .Where(p => p.Id == playlistId && p.UserId == ownerId)
                .Take(1)
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
