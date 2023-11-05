using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex.Results;

internal record TemporaryLinkResult
{
    [JsonPropertyName("href")]
    public required string Link { get; set; }
}
