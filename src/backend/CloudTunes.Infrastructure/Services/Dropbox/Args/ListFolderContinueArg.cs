using System.Text.Json.Serialization;

namespace CloudTunes.Infrastructure.Services.Dropbox.Args;

internal sealed record ListFolderContinueArg
{
    [JsonPropertyName("cursor")]
    public required string Cursor { get; init; }
}
