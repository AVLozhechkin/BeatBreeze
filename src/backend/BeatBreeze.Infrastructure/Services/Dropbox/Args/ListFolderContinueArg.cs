using System.Text.Json.Serialization;

namespace BeatBreeze.Infrastructure.Services.Dropbox.Args;

internal sealed record ListFolderContinueArg
{
    [JsonPropertyName("cursor")]
    public required string Cursor { get; init; }
}
