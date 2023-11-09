using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Core.DataProviders;

public record AccessToken
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
