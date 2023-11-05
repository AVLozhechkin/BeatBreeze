using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;

internal record ListFolderContinueArg
{
    [JsonPropertyName("cursor")]
    public required string Cursor { get; init; }
}
