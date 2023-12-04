using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces;

public interface IHistoryService
{
    Task<History> GetUserHistory(Guid userId);
    Task<History> AddToHistory(Guid userId, Guid songFileId);
}
