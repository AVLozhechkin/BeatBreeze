namespace CloudMusicPlayer.API.Dtos.Requests;

public record AddToHistoryRequest
{
    public Guid SongFileId { get; set; }
}
