namespace CloudMusicPlayer.Core.Models;

public record PlaylistItem : BaseEntity
{
    public PlaylistItem() {}

    public PlaylistItem(Guid playlistId, Guid songFileId)
    {
        PlaylistId = playlistId;
        SongFileId = songFileId;
    }


    public Playlist Playlist { get; set; } = null!;
    public Guid PlaylistId { get; set; }
    public SongFile SongFile { get; set; } = null!;
    public Guid SongFileId { get; set; }
    public DateTimeOffset AddedAt { get; set; }
}
