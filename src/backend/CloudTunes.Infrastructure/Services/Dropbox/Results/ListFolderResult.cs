using System.Text.Json.Serialization;

namespace CloudTunes.Infrastructure.Services.Dropbox.Results;

internal sealed record ListFolderResult
{
    [JsonPropertyName("entries")]
    public required IList<Metadata> Entries { get; set; }

    [JsonPropertyName("has_more")]
    public required bool HasMore { get; set; }

    [JsonPropertyName("cursor")]
    public required string Cursor { get; set; }
}
