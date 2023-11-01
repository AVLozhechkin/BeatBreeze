using System.Linq.Expressions;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace CloudMusicPlayer.Tests.Services;

public class PlaylistsServiceTests
{
    [Fact]
    public async Task GetPlaylistsByUserId_Should_ReturnUserPlaylists()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlists = new List<Playlist>();

        unitOfWorkMock
            .PlaylistRepository
            .GetAllByUserIdAsync(userId)
            .Returns(playlists);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.GetPlaylistsByUserId(userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetAllByUserIdAsync(userId);
        Assert.True(result.IsSuccess);
        Assert.Equal(playlists, result.Value);
    }

    [Fact]
    public async Task GetPlaylistById_Should_ReturnUserPlaylistById()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var playlist = new Playlist() { Id = Guid.NewGuid(), UserId = Guid.NewGuid()};

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlist.Id)
            .Returns(playlist);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.GetPlaylistById(playlist.Id, playlist.UserId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlist.Id);
        Assert.True(result.IsSuccess);
        Assert.Equal(playlist, result.Value);
    }

    [Fact]
    public async Task GetPlaylistById_Should_ReturnFailure_When_UserIsNotPlaylistOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var anotherUserId = Guid.NewGuid();
        var playlist = new Playlist() { Id = Guid.NewGuid(), UserId = Guid.NewGuid()};
        var failureMessage = "User is not the owner of the Playlist";

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlist.Id)
            .Returns(playlist);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.GetPlaylistById(playlist.Id, anotherUserId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlist.Id);
        Assert.True(result.IsFailure);
        Assert.Equal(failureMessage, result.Error);
    }

    [Fact]
    public async Task GetPlaylistById_Should_ReturnNull_When_RepositoryReturnsNull()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var playlist = new Playlist() { Id = Guid.NewGuid(), UserId = Guid.NewGuid()};

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlist.Id)
            .ReturnsNull();

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.GetPlaylistById(playlist.Id, playlist.UserId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlist.Id);
        Assert.True(result.IsSuccess);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task CreatePlaylist_Should_CreatePlaylist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var playlistName = "Playlist name";
        var userId = Guid.NewGuid();
        Expression<Predicate<Playlist>> playlistPredicate = p => p.UserId == userId && p.Name == playlistName;
        unitOfWorkMock
            .PlaylistRepository
            .AddAsync(Arg.Is(playlistPredicate), true)
            .Returns(Result.Success());

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.CreatePlaylist(userId, playlistName);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .AddAsync(Arg.Is(playlistPredicate), true);
        Assert.True(result.IsSuccess);
        Assert.Equal(playlistName, result.Value.Name);
        Assert.Equal(userId, result.Value.UserId);
    }

    [Fact]
    public async Task CreatePlaylist_Should_ReturnFailure_When_RepositoryFailsToAdd()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var playlistName = "Playlist name";
        var userId = Guid.NewGuid();
        var errorMessage = "To Infinity and Beyond!";
        Expression<Predicate<Playlist>> playlistPredicate = p => p.UserId == userId && p.Name == playlistName;

        unitOfWorkMock
            .PlaylistRepository
            .AddAsync(Arg.Is(playlistPredicate), true)
            .Returns(Result.Failure(errorMessage));

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.CreatePlaylist(userId, playlistName);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .AddAsync(Arg.Is(playlistPredicate), true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public async Task DeletePlaylist_Should_DeletePlaylist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();

        unitOfWorkMock
            .PlaylistRepository
            .RemoveAsync(playlistId, userId,true)
            .Returns(Result.Success());

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.DeletePlaylist(playlistId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .RemoveAsync(playlistId, userId, true);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task DeletePlaylist_Should_ReturnFailure_When_RepositoryFailsToDelete()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var errorMessage = "To Infinity and Beyond!";

        unitOfWorkMock
            .PlaylistRepository
            .RemoveAsync(playlistId, userId,true)
            .Returns(Result.Failure(errorMessage));

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.DeletePlaylist(playlistId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .RemoveAsync(playlistId, userId,true);
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task AddToPlaylist_Should_Add()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist() { UserId = userId, PlaylistItems = new List<PlaylistItem>()};
        Expression<Predicate<PlaylistItem>> playlistItemPredicate = pi => pi.SongFileId == songFileId && pi.PlaylistId == playlistId;

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);
        unitOfWorkMock
            .PlaylistItemRepository
            .AddAsync(
                Arg.Is(playlistItemPredicate),
                true)
            .Returns(Result.Success());

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.AddToPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        await unitOfWorkMock
            .PlaylistItemRepository
            .Received(1)
            .AddAsync(Arg.Is(playlistItemPredicate),
                true);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task AddToPlaylist_Should_ReturnFailure_When_PlaylistDoesntExist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .ReturnsNull();

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.AddToPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("Playlist was not found", result.Error);
    }

    [Fact]
    public async Task AddToPlaylist_Should_ReturnFailure_When_UserIsNotPlaylistOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist() { UserId = anotherUserId, PlaylistItems = new List<PlaylistItem>()};

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.AddToPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("User is not the owner of the Playlist", result.Error);
    }

    [Fact]
    public async Task AddToPlaylist_Should_ReturnFailure_When_RepositoryCantAdd()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist() { UserId = userId, PlaylistItems = new List<PlaylistItem>()};
        var errorMessage = "To Infinity and Beyond!";
        Expression<Predicate<PlaylistItem>> playlistItemPredicate = pi => pi.SongFileId == songFileId && pi.PlaylistId == playlistId;

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);
        unitOfWorkMock
            .PlaylistItemRepository
            .AddAsync(
                Arg.Is(playlistItemPredicate),
                true)
            .Returns(Result.Failure(errorMessage));

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.AddToPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        await unitOfWorkMock
            .PlaylistItemRepository
            .Received(1)
            .AddAsync(Arg.Is(playlistItemPredicate),
                true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public async Task RemoveFromPlaylist_Should_Remove()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlistItemId = Guid.NewGuid();
        var playlist = new Playlist
        {
            UserId = userId,
            PlaylistItems = new List<PlaylistItem>()
            {
                new PlaylistItem { Id = playlistItemId, SongFileId = songFileId }
            }
        };

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);
        unitOfWorkMock
            .PlaylistItemRepository
            .RemoveAsync(playlistItemId,true)
            .Returns(Result.Success());

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.RemoveFromPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        await unitOfWorkMock
            .PlaylistItemRepository
            .Received(1)
            .RemoveAsync(playlistItemId,true);
        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(result.Value.PlaylistItems, item => item.Id == playlistItemId);
    }

    [Fact]
    public async Task RemoveFromPlaylist_Should_ReturnFailure_When_PlaylistNotFound()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .ReturnsNull();

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.RemoveFromPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("Playlist was not found", result.Error);
    }

    [Fact]
    public async Task RemoveFromPlaylist_Should_ReturnFailure_When_UserIsNotPlaylistOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var anotherUserId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist
        {
            UserId = anotherUserId
        };

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.RemoveFromPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("User is not the owner of the Playlist", result.Error);
    }

    [Fact]
    public async Task RemoveFromPlaylist_Should_ReturnFailure_When_NoPlaylistItemInPlaylist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist
        {
            UserId = userId,
            PlaylistItems = new List<PlaylistItem>()
        };

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.RemoveFromPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        Assert.True(result.IsFailure);
        Assert.Equal("There is no such element in playlist", result.Error);
    }

    [Fact]
    public async Task RemoveFromPlaylist_Should_ReturnFailure_When_RepositoryCantRemove()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var userId = Guid.NewGuid();
        var playlistItemId = Guid.NewGuid();
        var playlistId = Guid.NewGuid();
        var songFileId = Guid.NewGuid();
        var playlist = new Playlist
        {
            UserId = userId,
            PlaylistItems = new List<PlaylistItem>()
            {
                new PlaylistItem
                {
                    Id = playlistItemId,
                    SongFileId = songFileId
                }
            }
        };
        var errorMessage = "To Infinity and Beyond!";

        unitOfWorkMock
            .PlaylistRepository
            .GetByIdAsync(playlistId)
            .Returns(playlist);

        unitOfWorkMock
            .PlaylistItemRepository
            .RemoveAsync(playlistItemId, true)
            .Returns(Result.Failure(errorMessage));

        var playlistService = new PlaylistService(unitOfWorkMock);

        // Act
        var result = await playlistService.RemoveFromPlaylist(playlistId, songFileId, userId);

        // Assert
        await unitOfWorkMock
            .PlaylistRepository
            .Received(1)
            .GetByIdAsync(playlistId, true);
        await unitOfWorkMock
            .PlaylistItemRepository
            .Received(1)
            .RemoveAsync(playlistItemId, true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }
}
