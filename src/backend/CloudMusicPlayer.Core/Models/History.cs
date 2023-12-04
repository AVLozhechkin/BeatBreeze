namespace CloudMusicPlayer.Core.Models;

public record History : BaseEntity
{
    public Guid UserId { get; init; }
    public User User { get; init; } = null!;
    public ICollection<HistoryItem> HistoryItems { get; init; } = null!;
}
