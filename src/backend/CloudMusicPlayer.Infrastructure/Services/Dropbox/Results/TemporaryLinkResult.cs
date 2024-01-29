using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.Services.Dropbox.Results;

internal sealed record TemporaryLinkResult
{
    [JsonPropertyName("link")]
    public required string Link { get; set; }
}
