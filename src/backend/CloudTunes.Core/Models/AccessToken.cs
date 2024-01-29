using System.Text.Json.Serialization;

namespace CloudTunes.Core.Models;


// TODO JsonPropertyName should not be here
public record AccessToken
{
    [JsonPropertyName("access_token")]
    public string Token { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
}
