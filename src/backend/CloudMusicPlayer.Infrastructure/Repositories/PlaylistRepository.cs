using CloudMusicPlayer.Core;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class PlaylistRepository : IPlaylistRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Result<Playlist?>> GetByIdAsync(Guid playlistId, bool includeSongFiles, bool asNoTracking = true)
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

        try
        {
            var playlist = await query.FirstOrDefaultAsync(pl => pl.Id == playlistId);

            return Result.Success<Playlist?>(playlist);
        }
        catch (Exception ex)
        {
            return Result.Failure<Playlist?>(DataLayerErrors.Database.GetError());
        }
    }

    public async Task<Result<List<Playlist>>> GetAllByUserIdAsync(Guid userId, bool includeSongFiles, bool asNoTracking = true)
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

        try
        {
            var playlists = await query.Where(pl => pl.UserId == userId).ToListAsync();

            return Result.Success(playlists);
        }
        catch (Exception ex)
        {
            return Result.Failure<List<Playlist>>(DataLayerErrors.Database.GetError());
        }
    }

    public async Task<Result> AddAsync(Playlist playlist, bool saveChanges = false)
    {
        await _applicationContext.Playlists.AddAsync(playlist);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult();
        }

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Playlist playlist, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeUpdated = _applicationContext.Playlists.Where(p => p.Id == playlist.Id);

            return await _applicationContext.ExecuteUpdateResult(toBeUpdated, s =>
                    s.SetProperty(p => p.Name, playlist.Name)
                        .SetProperty(p => p.UpdatedAt, playlist.UpdatedAt));
        }

        _applicationContext.Playlists.Update(playlist);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid playlistId, Guid ownerId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.Playlists
                .Where(p => p.Id == playlistId && p.UserId == ownerId).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted);
        }

        var playlist = await _applicationContext.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == ownerId);

        if (playlist is null)
        {
            return Result.Failure(DataLayerErrors.Database.NotFound());
        }

        _applicationContext.Playlists.Remove(playlist);

        return Result.Success();
    }
}
