using System.ComponentModel.DataAnnotations;

namespace CloudMusicPlayer.Core.Models;

public record PlaylistItem
{
    [Key]
    public Guid Id { get; set; }

    public Playlist Playlist { get; set; } = null!;
    public Guid PlaylistId { get; set; }
    public SongFile SongFile { get; set; } = null!;
    public Guid SongFileId { get; set; }
    public DateTimeOffset AddedAt { get; set; }
}
