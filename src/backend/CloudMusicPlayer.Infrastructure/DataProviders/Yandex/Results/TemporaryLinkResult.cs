using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex.Results;

public class TemporaryLinkResult
{
    [JsonPropertyName("href")]
    public required string Link { get; set; }
}
