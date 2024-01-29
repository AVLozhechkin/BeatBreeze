using CloudTunes.Core.Exceptions;

namespace CloudTunes.Core.Models;

public record Playlist : BaseEntity
{
    public Playlist() { }

    public Playlist(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            ValidationException.Create(nameof(name), "Playlist name must be not empty or whitespace");
        }

        UserId = userId;
        CreatedAt = DateTimeOffset.UtcNow;
        UpdatedAt = DateTimeOffset.UtcNow;
        Name = name;
        Size = 0;
    }

    public string Name { get; init; } = null!;
    public User? User { get; init; }
    public Guid UserId { get; init; }
    public int Size { get; set; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; set; }
    public IList<PlaylistItem> PlaylistItems { get; init; } = null!;
}
