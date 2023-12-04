using CloudMusicPlayer.Core.Exceptions;

namespace CloudMusicPlayer.Core.Models;

public record Playlist : BaseEntity
{
    public Playlist() {}

    public Playlist(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Playlist name must be not empty or whitespace");
        }

        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
        Name = name;
    }

    public string Name { get; init; } = null!;
    public User? User { get; init; }
    public Guid UserId { get; init; }
    public int Size { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public ICollection<PlaylistItem> PlaylistItems { get; init; } = null!;
}
