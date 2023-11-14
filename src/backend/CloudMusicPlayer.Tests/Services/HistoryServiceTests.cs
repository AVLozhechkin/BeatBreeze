using System.Linq.Expressions;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.UnitOfWorks;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace CloudMusicPlayer.Tests.Services;

public class HistoryServiceTests
{
    [Fact]
    public async Task GetUserHistory_Should_ReturnHistory()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var history = new History() { UserId = Guid.NewGuid() };
        var historyResult =

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(Arg.Is(history.UserId), true, true)
            .Returns(history);

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.GetUserHistory(history.UserId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(history.UserId, true, true);
        Assert.True(result.IsSuccess);
        Assert.Equal(history, result.Value);
    }

    [Fact]
    public async Task GetUserHistory_Should_ReturnNull_When_RepositoryReturnsNull()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(Arg.Is(userId), true,true)
            .ReturnsNull();

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.GetUserHistory(userId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(userId, true);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task AddToHistory_Should_AddHistoryItem()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var history = new History() { Id = Guid.NewGuid(), UserId = userId, HistoryItems = new List<HistoryItem>()};
        var historyItem = new HistoryItem { HistoryId = history.Id, SongFileId = userId };

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(userId, true)
            .Returns(history);
        unitOfWorkMock
            .HistoryItemRepository
            .AddAsync(historyItem, true)
            .Returns(Result.Success());

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.AddToHistory(userId, songFileId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(userId, true);
        await unitOfWorkMock
            .HistoryItemRepository
            .Received(1)
            .AddAsync(
                Arg.Is<HistoryItem>(hi => hi.HistoryId == history.Id && hi.SongFileId == songFileId),
                true);
        Assert.True(result.IsSuccess);
        Assert.Contains(history.HistoryItems,
            item => item.HistoryId == history.Id
                    && item.SongFileId == songFileId
                    && item.AddedAt != default);
        Assert.Equal(history, result.Value);
    }

    [Fact]
    public async Task AddToHistory_Should_ReturnFailure_When_HistoryNotExist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(userId, true)
            .ReturnsNull();

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.AddToHistory(userId, songFileId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(userId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("History was not found", result.Error);
    }

    [Fact]
    public async Task AddToHistory_Should_ReturnFailure_When_UserIsNotHistoryOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var someonesHistory = new History { UserId = Guid.NewGuid() };

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(userId, true)
            .Returns(someonesHistory);

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.AddToHistory(userId, songFileId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(userId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("User is not the owner of the History", result.Error);
    }

    [Fact]
    public async Task AddToHistory_Should_ReturnFailure_When_UserRepositoryReturnsFailure()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var history = new History() { Id = Guid.NewGuid(), UserId = userId };
        var errorMessage = "To Infinity and Beyond!";
        Expression<Predicate<HistoryItem>>
            historyItemPtedicate = hi => hi.HistoryId == history.Id && hi.SongFileId == songFileId;

        unitOfWorkMock
            .HistoryRepository
            .GetByUserIdAsync(userId, true)
            .Returns(history);
        unitOfWorkMock
            .HistoryItemRepository
            .AddAsync(Arg.Is(historyItemPtedicate), true)
            .Returns(Result.Failure(errorMessage));

        var historyService = new HistoryService(unitOfWorkMock);

        // Act
        var result = await historyService.AddToHistory(userId, songFileId);

        // Assert
        await unitOfWorkMock
            .HistoryRepository
            .Received(1)
            .GetByUserIdAsync(userId, true);
        await unitOfWorkMock
            .HistoryItemRepository
            .Received(1)
            .AddAsync(
                Arg.Is(historyItemPtedicate)
                , true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }
}
