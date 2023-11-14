using CloudMusicPlayer.Core;
using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CloudMusicPlayer.Infrastructure.Errors;
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
            return await _applicationContext.SaveChangesResult();
        }

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid playlistItemId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.PlaylistItems
                .Where(pi => pi.Id == playlistItemId).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted);
        }

        try
        {
            var playlistItem =
                await _applicationContext.PlaylistItems.FirstOrDefaultAsync(pi => pi.Id == playlistItemId);

            if (playlistItem is null)
            {
                return Result.Failure(DataLayerErrors.Database.NotFound());
            }

            _applicationContext.PlaylistItems.Remove(playlistItem);

            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(DataLayerErrors.Database.DeleteError());
        }
    }
}
