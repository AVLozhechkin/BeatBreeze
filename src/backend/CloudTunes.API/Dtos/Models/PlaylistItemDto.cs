using CloudTunes.Core.Models;

namespace CloudTunes.API.Dtos.Models;

public class PlaylistItemDto
{
    public Guid Id { get; private set; }
    public DateTimeOffset AddedAt { get; private set; }
    public MusicFileDto MusicFile { get; private set; } = null!;

    public static PlaylistItemDto Create(PlaylistItem playlistItem)
    {
        ArgumentNullException.ThrowIfNull(playlistItem);

        return new PlaylistItemDto()
        {
            Id = playlistItem.Id,
            MusicFile = MusicFileDto.Create(playlistItem.MusicFile),
            AddedAt = playlistItem.AddedAt
        };
    }
}
