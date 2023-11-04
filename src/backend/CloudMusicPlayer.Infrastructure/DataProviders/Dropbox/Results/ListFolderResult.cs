using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;

public class ListFolderResult
{
    [JsonPropertyName("entries")]
    public IList<Metadata> Entries { get; set; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("cursor")]
    public string Cursor { get; set; }
}

public class Metadata
{
    [JsonPropertyName(".tag")]
    public required string Tag { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("path_display")]
    public required string PathDisplay { get; set; }

    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("size")]
    public required ulong Size { get; set; }

    [JsonPropertyName("content_hash")]
    public required string ContentHash { get; set; }
}
