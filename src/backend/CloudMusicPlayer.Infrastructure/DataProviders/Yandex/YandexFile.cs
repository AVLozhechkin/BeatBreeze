using System.Text.Json.Serialization;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex;

public record YandexFile
{
    [JsonPropertyName("size")]
    public int Size { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("mime_type")]
    public required string MimeType { get; set; }

    [JsonPropertyName("file")]
    public required string FileUrl { get; set; }

    [JsonPropertyName("path")]
    public required string Path { get; set; }
    [JsonPropertyName("md5")]
    public required string Md5 { get; set; }
    [JsonPropertyName("resource_id")]
    public required string ResourceId { get; set; }


    public SongFile MapToSongFile(DataProvider dataProvider)
    {

        return new SongFile
        {
            Name = Name,
            Path = Path,
            Hash = Md5,
            FileId = ResourceId,
            Type = MimeType,
            Url = FileUrl,
            Size = Size,
            DataProvider = dataProvider
        };
    }
}
