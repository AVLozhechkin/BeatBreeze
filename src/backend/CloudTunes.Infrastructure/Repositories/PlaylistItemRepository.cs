using CloudTunes.Core.Exceptions;
using CloudTunes.Core.Interfaces.Repositories;
using CloudTunes.Core.Models;
using CloudTunes.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudTunes.Infrastructure.Repositories;

internal sealed class PlaylistItemRepository : IPlaylistItemRepository
{
    private readonly ApplicationContext _applicationContext;

    public PlaylistItemRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task AddAsync(PlaylistItem playlistItem, bool saveChanges)
    {
        await _applicationContext
            .PlaylistItems
            .AddAsync(playlistItem);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAsync(PlaylistItem playlistItem, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .PlaylistItems
                .Where(pi => pi.Id == playlistItem.Id)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<PlaylistItem>(playlistItem.Id);
            }

            return;
        }

        _applicationContext.PlaylistItems.Remove(playlistItem);
    }
}
