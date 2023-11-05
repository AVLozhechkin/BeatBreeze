using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;

internal record ListFolderResult
{
    [JsonPropertyName("entries")]
    public IList<Metadata> Entries { get; set; }

    [JsonPropertyName("hasMore")]
    public bool HasMore { get; set; }

    [JsonPropertyName("cursor")]
    public string Cursor { get; set; }
}
