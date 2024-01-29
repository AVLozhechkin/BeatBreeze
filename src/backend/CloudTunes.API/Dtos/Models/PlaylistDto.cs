using System.Text.Json.Serialization;
using CloudTunes.Core.Models;

namespace CloudTunes.API.Dtos.Models;

public record PlaylistDto
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public int Size { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IEnumerable<PlaylistItemDto>? PlaylistItems { get; set; }

    public static PlaylistDto Create(Playlist playlist)
    {
        ArgumentNullException.ThrowIfNull(playlist);

        return new PlaylistDto
        {
            Id = playlist.Id,
            Name = playlist.Name,
            CreatedAt = playlist.CreatedAt,
            UpdatedAt = playlist.UpdatedAt,
            Size = playlist.Size,
            PlaylistItems = playlist.PlaylistItems is null ? null : playlist.PlaylistItems.Select(PlaylistItemDto.Create)
        };
    }
}
