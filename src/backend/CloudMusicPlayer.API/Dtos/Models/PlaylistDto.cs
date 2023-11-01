using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record PlaylistDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required int Size { get; set; }
    public required DateTimeOffset CreatedAt { get; set; }
    public required DateTimeOffset UpdatedAt { get; set; }
    public required IEnumerable<SongFileDto> SongFiles { get; set; }

    public static PlaylistDto Create(Playlist playlist)
    {
        return new PlaylistDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            UpdatedAt = playlist.UpdatedAt,
            Size = playlist.PlaylistItems.Count,
            SongFiles = playlist.PlaylistItems.Select(pi => SongFileDto.Create(pi.SongFile))
        };
    }
}
