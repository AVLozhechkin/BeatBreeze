using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Models;
using Microsoft.Extensions.Logging;

namespace CloudMusicPlayer.Core.Services;

internal sealed class HistoryService : IHistoryService
{
    private readonly ILogger<HistoryService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HistoryService(ILogger<HistoryService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<History> GetUserHistory(Guid userId)
    {
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true, true);

        if (history is null)
        {
            _logger.LogWarning("There is no history for the user ({userId})", userId);
            throw NotFoundException.Create<History>();
        }

        return history;
    }

    public async Task<History> AddToHistory(Guid userId, Guid songFileId)
    {
        var history = await _unitOfWork.HistoryRepository.GetByUserIdAsync(userId, true, true);

        if (history is null)
        {
            _logger.LogWarning("There is no history for the user ({userId}), " +
                               "songFile ({songFileId}) was not added", userId, songFileId);
            throw NotFoundException.Create<History>();
        }

        var historyItem = new HistoryItem(history.Id, songFileId);

        await _unitOfWork.HistoryItemRepository.AddAsync(historyItem, true);

        history.HistoryItems.Add(historyItem);

        return history;
    }
}
