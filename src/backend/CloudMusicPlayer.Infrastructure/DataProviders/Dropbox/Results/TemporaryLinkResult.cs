using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;

internal record TemporaryLinkResult
{
    [JsonPropertyName("link")]
    public required string Link { get; set; }
}
