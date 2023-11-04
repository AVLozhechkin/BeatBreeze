namespace CloudMusicPlayer.Core.Models;

public record AccessToken
{
    public required string Token { get; set; }
    public required DateTimeOffset ExpiresAt { get; set; }
}
