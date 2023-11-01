using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

public class PlaylistRepository : IPlaylistRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Playlist?> GetByIdAsync(Guid playlistId, bool asNoTracking = true)
    {
        var query = _applicationContext.Playlists
            .Include(pl => pl.PlaylistItems)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(pl => pl.Id == playlistId);
    }

    public async Task<List<Playlist>> GetAllByUserIdAsync(Guid userId, bool asNoTracking = true)
    {
        var query = _applicationContext.Playlists
            .Include(pl => pl.PlaylistItems)
                .ThenInclude(pi => pi.SongFile)
            .AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.Where(pl => pl.UserId == userId).ToListAsync();
    }

    public async Task<Result> AddAsync(Playlist playlist, bool saveChanges = false)
    {
        if (saveChanges)
        {
            await _applicationContext.Playlists.AddAsync(playlist);

            return await _applicationContext.SaveChangesResult("Playlist was not added");
        }

        await _applicationContext.Playlists.AddAsync(playlist);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(Playlist playlist, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeUpdated = _applicationContext.Playlists.Where(p => p.Id == playlist.Id);

            return await _applicationContext.ExecuteUpdateResult(toBeUpdated, s =>
                    s.SetProperty(p => p.Name, playlist.Name)
                        .SetProperty(p => p.UpdatedAt, playlist.UpdatedAt),
                "Data provider was not updated");
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

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted, "Playlist was not deleted");
        }

        var playlist = await _applicationContext.Playlists.FirstOrDefaultAsync(p => p.Id == playlistId && p.UserId == ownerId);

        if (playlist is null)
        {
            return Result.Failure("Playlist was not found");
        }

        _applicationContext.Playlists.Remove(playlist);

        return Result.Success();
    }
}
