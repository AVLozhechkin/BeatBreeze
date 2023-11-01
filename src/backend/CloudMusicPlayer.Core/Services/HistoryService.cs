using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Services;

public class HistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public HistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<History?>> GetUserHistory(Guid userId)
    {
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        return Result.Success(history);
    }

    public async Task<Result<History>> AddToHistory(Guid userId, Guid songFileId)
    {
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        if (history is null)
        {
            return Result.Failure<History>("History was not found");
        }

        if (history.UserId != userId)
        {
            return Result.Failure<History>("User is not the owner of the History");
        }

        var item = new HistoryItem() { HistoryId = history.Id, SongFileId = songFileId, AddedAt = DateTimeOffset.UtcNow };

        var result = await _unitOfWork.HistoryItemRepository.AddAsync(item, true);

        if (result.IsFailure)
        {
            return Result.Failure<History>(result.Error);
        }

        history.HistoryItems.Add(item);

        return Result.Success(history);
    }
}
