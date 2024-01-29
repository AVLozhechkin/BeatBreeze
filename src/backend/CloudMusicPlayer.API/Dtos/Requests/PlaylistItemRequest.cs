namespace CloudMusicPlayer.API.Dtos.Requests;

public record PlaylistItemRequest
{
    public required Guid MusicFileId { get; set; }
}
