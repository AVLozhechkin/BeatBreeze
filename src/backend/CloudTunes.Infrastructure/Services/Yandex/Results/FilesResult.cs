using System.Text.Json.Serialization;

namespace CloudTunes.Infrastructure.Services.Yandex.Results;

internal sealed record FilesResult
{
    [JsonPropertyName("items")]
    public IReadOnlyList<YandexFile> Items { get; set; } = null!;

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("limit")]
    public int Limit { get; set; }
}
