using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;

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
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        return Result.Success(history);
    }

    public async Task<Result<History>> AddToHistory(Guid userId, Guid songFileId)
    {
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true);

        if (history is null)
        {
            return Result.Failure<History>(DomainErrors.NotFound);
        }

        if (history.UserId != userId)
        {
            return Result.Failure<History>(DomainErrors.NotTheOwner);
        }

        var itemCreationResult = HistoryItem.Create(history.Id, songFileId);

        if (itemCreationResult.IsFailure)
        {
            return Result.Failure<History>(DomainErrors.CantBeCreated);
        }

        var addResult = await _unitOfWork.HistoryItemRepository.AddAsync(itemCreationResult.Value, true);

        if (addResult.IsFailure)
        {
            return Result.Failure<History>(addResult.Error);
        }

        history.HistoryItems.Add(itemCreationResult.Value);

        return Result.Success(history);
    }
}
