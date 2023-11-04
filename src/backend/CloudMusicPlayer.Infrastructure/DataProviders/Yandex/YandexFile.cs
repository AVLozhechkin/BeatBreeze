using System.Text.Json.Serialization;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Infrastructure.DataProviders.Yandex;

public record YandexFile
{
    [JsonPropertyName("size")]
    public uint Size { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

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
            Type = GetAudioType(),
            Size = Size,
            DataProvider = dataProvider
        };
    }

    private AudioTypes GetAudioType()
    {
        if (Path.EndsWith("flac"))
        {
            return AudioTypes.Flac;
        }

        if (Path.EndsWith("mp3"))
        {
            return AudioTypes.Mp3;
        }

        return AudioTypes.Unknown;
    }
}
