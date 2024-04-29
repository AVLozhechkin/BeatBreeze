using System.Text.Json.Serialization;

namespace BeatBreeze.Infrastructure.Services.Dropbox.Args;

internal sealed record TemporaryLinkArg
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }
}
