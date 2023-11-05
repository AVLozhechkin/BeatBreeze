using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;

internal record TemporaryLinkArg
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }
}
