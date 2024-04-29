namespace BeatBreeze.API.Dtos.Requests;

public record CreatePlaylistRequest
{
    public required string Name { get; set; }
}
