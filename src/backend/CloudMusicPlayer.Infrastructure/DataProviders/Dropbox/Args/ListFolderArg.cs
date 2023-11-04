using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Args;

public record ListFolderArg
{
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    [JsonPropertyName("limit")]
    public required int Limit { get; init; }

    [JsonPropertyName("recursive")]
    public required bool Recursive { get; init; }
}
