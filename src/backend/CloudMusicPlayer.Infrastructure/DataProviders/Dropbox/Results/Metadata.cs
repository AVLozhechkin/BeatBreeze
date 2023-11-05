using System.Text.Json.Serialization;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Dropbox.Results;

internal record Metadata
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
