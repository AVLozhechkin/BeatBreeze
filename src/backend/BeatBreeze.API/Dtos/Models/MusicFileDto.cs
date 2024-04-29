using BeatBreeze.Core.Enums;
using BeatBreeze.Core.Models;

namespace BeatBreeze.API.Dtos.Models;

public class MusicFileDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required AudioTypes Type { get; set; }
    public required ulong Size { get; set; }

    public static MusicFileDto Create(MusicFile musicFile)
    {
        ArgumentNullException.ThrowIfNull(musicFile);

        return new MusicFileDto
        {
            Id = musicFile.Id,
            Name = musicFile.Name,
            Path = musicFile.Path,
            Type = musicFile.Type,
            Size = musicFile.Size
        };
    }
}
