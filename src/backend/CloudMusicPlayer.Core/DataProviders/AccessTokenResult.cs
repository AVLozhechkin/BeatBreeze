using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Core.DataProviders;

public record AccessTokenResult
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
