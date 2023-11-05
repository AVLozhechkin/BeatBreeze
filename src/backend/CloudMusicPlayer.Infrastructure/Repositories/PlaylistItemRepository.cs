using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class PlaylistItemRepository : IPlaylistItemRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistItemRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Result> AddAsync(PlaylistItem playlistItem, bool saveChanges = false)
    {
        await _applicationContext.PlaylistItems.AddAsync(playlistItem);

        if (saveChanges)
        {
            return await _applicationContext.SaveChangesResult("Playlist Item was not added");
        }

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid playlistItemId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.PlaylistItems
                .Where(pi => pi.Id == playlistItemId).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted, "Playlist Item was not deleted");
        }

        var playlistItem = await _applicationContext.PlaylistItems.FirstOrDefaultAsync(pi => pi.Id == playlistItemId);

        if (playlistItem is null)
        {
            return Result.Failure("Playlist Item was not found");
        }

        _applicationContext.PlaylistItems.Remove(playlistItem);

        return Result.Success();
    }
}
