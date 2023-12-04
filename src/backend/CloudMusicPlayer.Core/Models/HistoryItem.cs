namespace CloudMusicPlayer.Core.Models;

public record HistoryItem : BaseEntity
{
    public HistoryItem() {}

    public HistoryItem(Guid historyId, Guid songFileId)
    {
        HistoryId = historyId;
        SongFileId = songFileId;
        AddedAt = DateTimeOffset.UtcNow;
    }

    public Guid HistoryId { get; init; }
    public History History { get; init; } = null!;
    public Guid SongFileId { get; init; }
    public SongFile SongFile { get; init; } = null!;
    public DateTimeOffset AddedAt { get; init; }
}
