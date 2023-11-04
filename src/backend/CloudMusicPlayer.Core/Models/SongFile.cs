using System.ComponentModel.DataAnnotations;

namespace CloudMusicPlayer.Core.Models;

public record SongFile
{
    [Key]
    public Guid Id { get; set; }
    public DataProvider? DataProvider { get; set; }
    public Guid DataProviderId { get; set; }
    public required string Name { get; set; }
    public required string FileId { get; set; }
    public required string Hash { get; set; }
    public required string Path { get; set; }
    public required AudioTypes Type { get; set; }
    public ulong Size { get; set; }
}
