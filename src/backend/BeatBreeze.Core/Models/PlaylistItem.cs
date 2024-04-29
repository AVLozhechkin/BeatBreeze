namespace BeatBreeze.Core.Models;

public record PlaylistItem : BaseEntity
{
    public PlaylistItem() { }

    public PlaylistItem(Guid playlistId, Guid musicFileId)
    {
        PlaylistId = playlistId;
        MusicFileId = musicFileId;
        AddedAt = DateTime.UtcNow;
    }

    public PlaylistItem(Guid playlistId, MusicFile musicFile)
    {
        PlaylistId = playlistId;
        MusicFile = musicFile;
        AddedAt = DateTime.UtcNow;
    }


    public Playlist Playlist { get; set; } = null!;
    public Guid PlaylistId { get; set; }
    public MusicFile MusicFile { get; set; } = null!;
    public Guid MusicFileId { get; set; }
    public DateTimeOffset AddedAt { get; set; }
}
