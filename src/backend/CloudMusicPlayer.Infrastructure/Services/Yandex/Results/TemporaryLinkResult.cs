using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.Services.Yandex.Results;

internal sealed record TemporaryLinkResult
{
    [JsonPropertyName("href")]
    public required string Link { get; set; }
}
