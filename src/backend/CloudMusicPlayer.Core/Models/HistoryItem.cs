using CloudMusicPlayer.Core.Errors;

namespace CloudMusicPlayer.Core.Models;

public record HistoryItem
{
    public Guid Id { get; init; }
    public Guid HistoryId { get; set; }
    public History History { get; set; } = null!;
    public Guid SongFileId { get; set; }
    public SongFile SongFile { get; set; } = null!;
    public DateTimeOffset AddedAt { get; set; }

    public static Result<HistoryItem> Create(Guid historyId, Guid songFileId)
    {
        var history = new HistoryItem()
        {
            HistoryId = historyId,
            SongFileId = songFileId,
            AddedAt = DateTimeOffset.UtcNow
        };

        return Result.Success(history);
    }
}
