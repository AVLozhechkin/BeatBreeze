using System.Text.Json.Serialization;
using CloudTunes.Core.Enums;
using CloudTunes.Core.Models;

namespace CloudTunes.Infrastructure.Services.Yandex;

internal sealed record YandexFile
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


    public MusicFile MapToMusicFile()
    {
        return new MusicFile
        {
            Name = Name,
            Path = Path,
            Hash = Md5,
            FileId = ResourceId,
            Type = GetAudioType(),
            Size = Size,
        };
    }

    private AudioTypes GetAudioType()
    {
        if (Path.EndsWith("flac", StringComparison.InvariantCultureIgnoreCase))
        {
            return AudioTypes.Flac;
        }

        if (Path.EndsWith("mp3", StringComparison.InvariantCultureIgnoreCase))
        {
            return AudioTypes.Mp3;
        }

        return AudioTypes.Unknown;
    }
}
