using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class PlaylistItemRepository : IPlaylistItemRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistItemRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task AddAsync(PlaylistItem playlistItem, bool saveChanges = false)
    {
        await _applicationContext
            .PlaylistItems
            .AddAsync(playlistItem);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAsync(Guid playlistItemId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .PlaylistItems
                .Where(pi => pi.Id == playlistItemId)
                .Take(1)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<PlaylistItem>(playlistItemId);
            }

            return;
        }

        var playlistItem = new PlaylistItem() { Id = playlistItemId };
        _applicationContext.PlaylistItems.Remove(playlistItem);
    }
}
