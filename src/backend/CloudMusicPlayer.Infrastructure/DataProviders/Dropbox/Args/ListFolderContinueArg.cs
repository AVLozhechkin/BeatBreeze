using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;

public record ListFolderContinueArg
{
    [JsonPropertyName("cursor")]
    public required string Cursor { get; init; }
}
