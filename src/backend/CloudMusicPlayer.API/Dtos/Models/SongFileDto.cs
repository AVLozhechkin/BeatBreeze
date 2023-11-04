using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public class SongFileDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public required AudioTypes Type { get; set; }
    public required ulong Size { get; set; }

    public static SongFileDto Create(SongFile songFile)
    {
        return new SongFileDto
        {
            Id = songFile.Id,
            Name = songFile.Name,
            Path = songFile.Path,
            Type = songFile.Type,
            Size = songFile.Size
        };
    }
}
