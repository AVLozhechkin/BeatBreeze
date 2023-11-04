using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;

public record TemporaryLinkArg
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }
}
