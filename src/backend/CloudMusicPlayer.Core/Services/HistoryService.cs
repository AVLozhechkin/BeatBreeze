using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;

namespace CloudMusicPlayer.Core.Services;

public sealed class HistoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public HistoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<History?>> GetUserHistory(Guid userId)
    {
        var historyResult = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        return historyResult;
    }

    public async Task<Result<History>> AddToHistory(Guid userId, Guid songFileId)
    {
        var historyResult = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        if (historyResult.IsFailure)
        {
            return historyResult;
        }

        if (historyResult.Value is null)
        {
            return Result.Failure<History>(DomainLayerErrors.NotFound());
        }

        if (historyResult.Value.UserId != userId)
        {
            return Result.Failure<History>(DomainLayerErrors.NotTheOwner());
        }

        var itemCreationResult = HistoryItem.Create(historyResult.Value.Id, songFileId);

        if (itemCreationResult.IsFailure)
        {
            return Result.Failure<History>(itemCreationResult.Error);
        }

        var addResult = await _unitOfWork.HistoryItemRepository.AddAsync(itemCreationResult.Value, true);

        if (addResult.IsFailure)
        {
            return Result.Failure<History>(addResult.Error);
        }

        historyResult.Value.HistoryItems.Add(itemCreationResult.Value);

        return Result.Success(historyResult.Value);
    }
}
