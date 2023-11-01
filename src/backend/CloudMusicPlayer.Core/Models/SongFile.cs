using System.ComponentModel.DataAnnotations;

namespace CloudMusicPlayer.Core.Models;

public record SongFile
{
    [Key]
    public Guid Id { get; init; }
    public DataProvider? DataProvider { get; init; }
    public Guid DataProviderId { get; init; }
    public required string Name { get; init; }
    public required string FileId { get; init; }
    public required string Hash { get; init; }
    public required string Url { get; set; }
    public required string Path { get; init; }
    public required string Type { get; init; }
    public int Size { get; init; }
}
