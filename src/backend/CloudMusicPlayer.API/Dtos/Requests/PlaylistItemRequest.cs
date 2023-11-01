namespace CloudMusicPlayer.API.Dtos.Requests;

public record PlaylistItemRequest
{
    public required Guid PlaylistId { get; set; }
    public required Guid SongFileId { get; set; }
}
