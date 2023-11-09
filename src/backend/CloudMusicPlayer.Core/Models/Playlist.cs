using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Models;

public class Playlist
{
    public Guid Id { get; init; }

    public string Name { get; init; } = null!;

    public User? User { get; init; }
    public Guid UserId { get; init; }

    public int Size { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
    public ICollection<PlaylistItem> PlaylistItems { get; init; } = null!;

    public static Result<Playlist> Create(Guid userId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Result.Failure<Playlist>("Playlist name must not be a null or whitespace");
        }

        var playlist = new Playlist
        {
            UserId = userId,
            CreatedAt = DateTimeOffset.UtcNow,
            Name = name
        };

        return Result.Success(playlist);
    }
}
