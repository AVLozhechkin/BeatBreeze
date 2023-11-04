using System.Linq.Expressions;
using CloudMusicPlayer.Core.DataProviders;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;

namespace CloudMusicPlayer.Tests.Services;

public class ProviderServiceTests
{
    [Fact]
    public async Task GetAllProvidersByUserId_Should_ReturnProvidersFromRepository()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategies = Substitute.For<IEnumerable<IExternalProviderService>>();
        var expectedProviders = new List<DataProvider>();
        var userId = Guid.NewGuid();

        unitOfWorkMock.DataProviderRepository.GetAllByUserIdAsync(userId, true).Returns(expectedProviders);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providers = await service.GetAllProvidersByUserId(userId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).GetAllByUserIdAsync(userId, true);
        Assert.True(providers.IsSuccess);
        Assert.Equal(expectedProviders, providers.Value);
    }

    [Fact]
    public async Task GetDataProvider_Should_ReturnProviderFromRepository()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategies = Substitute.For<IEnumerable<IExternalProviderService>>();
        var userId = Guid.NewGuid();
        var expectedProvider = new DataProvider()
        {
            UserId = userId
        };
        var providerId = Guid.NewGuid();

        unitOfWorkMock.DataProviderRepository.GetAsync(providerId, true).Returns(expectedProvider);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.GetDataProvider(providerId, userId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).GetAsync(providerId, true);
        Assert.True(providerResult.IsSuccess);
        Assert.Equal(expectedProvider, providerResult.Value);
    }

    [Fact]
    public async Task GetDataProvider_Should_ReturnNull_When_RepositoryReturnsNull()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategies = Substitute.For<IEnumerable<IExternalProviderService>>();
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();

        unitOfWorkMock.DataProviderRepository.GetAsync(providerId, true).ReturnsNull();

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providers = await service.GetDataProvider(providerId, userId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).GetAsync(providerId, true);
        Assert.True(providers.IsSuccess);
        Assert.Null(providers.Value);
    }

    [Fact]
    public async Task GetDataProvider_Should_ReturnFailure_When_UserIsNotDataProviderOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategies = Substitute.For<IEnumerable<IExternalProviderService>>();
        var expectedProvider = new DataProvider();
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();

        unitOfWorkMock.DataProviderRepository.GetAsync(providerId, true).Returns(expectedProvider);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.GetDataProvider(providerId, userId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).GetAsync(providerId, true);
        Assert.True(providerResult.IsFailure);
        Assert.Equal("User is not the owner of the DataProvider", providerResult.Error);
    }

    [Theory]
    [InlineData(ProviderTypes.Dropbox)]
    [InlineData(ProviderTypes.Yandex)]
    [InlineData(ProviderTypes.Google)]
    public async Task AddDataProvider_Should_Add(ProviderTypes providerType)
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategy = Substitute.For<IExternalProviderService>();
        var strategies = new List<IExternalProviderService>() { strategy };
        var userId = Guid.NewGuid();
        var providerName = "Test providerName";
        var apiToken = "Test api token";
        var refreshToken = "Test refresh token";
        var expiresAt = DateTimeOffset.UtcNow.ToString();
        Expression<Predicate<DataProvider>> dataProviderPredicate = provider =>
            provider.Name == providerName &&
            provider.AccessToken.Token == apiToken &&
            provider.RefreshToken == refreshToken &&
            provider.ProviderType == providerType &&
            provider.UpdatedAt != default &&
            provider.AddedAt != default &&
            provider.UserId == userId;
        var songFiles = new SongFile[1];


        unitOfWorkMock
            .DataProviderRepository
            .AddAsync(Arg.Is(dataProviderPredicate))
            .Returns(Result.Success());
        unitOfWorkMock
            .SongFileRepository
            .AddRangeAsync(songFiles)
            .Returns(Result.Success());
        unitOfWorkMock
            .CommitAsync()
            .Returns(Result.Success());
        strategy
            .CanBeExecuted(Arg.Is<ProviderTypes>(pt => pt == providerType))
            .Returns(true);
        strategy
            .GetSongFiles(Arg.Is(dataProviderPredicate))
            .Returns(songFiles);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.AddDataProvider(
            providerType,
            userId,
            providerName,
            apiToken,
            refreshToken,
            expiresAt);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .AddAsync(Arg.Is(dataProviderPredicate), false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(1)
            .AddRangeAsync(songFiles, false);
        await unitOfWorkMock
            .Received(1)
            .CommitAsync();
        strategy
            .Received(1)
            .CanBeExecuted(providerType);
        await strategy
            .Received(1)
            .GetSongFiles(Arg.Is(dataProviderPredicate));
        Assert.True(providerResult.IsSuccess);
    }

    [Theory]
    [InlineData(null, "", "Api token is null or whitespace")]
    [InlineData("", "", "Api token is null or whitespace")]
    [InlineData(" ", "", "Api token is null or whitespace")]
    [InlineData("test", "", "Api token expiring time can't be parsed")]
    public async Task AddDataProvider_Should_ReturnFailure_When_DataProviderCantBeCreated(string apiToken, string expiresAt, string expectedMessage)
    {
        // Arrange
        var userId = Guid.NewGuid();
        var providerName = "Test providerName";
        var refreshToken = "Test refresh token";

        var service = new ProviderService(null!, null!);

        // Act
        var providerResult = await service.AddDataProvider(
            ProviderTypes.Dropbox,
            userId,
            providerName,
            apiToken,
            refreshToken,
            expiresAt);

        // Assert
        Assert.True(providerResult.IsFailure);
        Assert.Equal(expectedMessage, providerResult.Error);
    }

    [Fact]
    public async Task AddDataProvider_Should_ReturnFailure_When_UnitOfWorkFails()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var strategy = Substitute.For<IExternalProviderService>();
        var strategies = new List<IExternalProviderService>() { strategy };
        var userId = Guid.NewGuid();
        var providerName = "Test providerName";
        var apiToken = "Test api token";
        var refreshToken = "Test refresh token";
        var expiresAt = DateTimeOffset.UtcNow.ToString();
        Expression<Predicate<DataProvider>> dataProviderPredicate = provider =>
            provider.Name == providerName &&
            provider.AccessToken.Token == apiToken &&
            provider.RefreshToken == refreshToken &&
            provider.ProviderType == ProviderTypes.Yandex &&
            provider.UpdatedAt != default &&
            provider.AddedAt != default &&
            provider.UserId == userId;
        var songFiles = new SongFile[1];
        var errorMessage = "To Infinity and Beyond!";

        unitOfWorkMock
            .DataProviderRepository
            .AddAsync(Arg.Is(dataProviderPredicate))
            .Returns(Result.Success());
        unitOfWorkMock
            .SongFileRepository
            .AddRangeAsync(songFiles)
            .Returns(Result.Success());
        unitOfWorkMock
            .CommitAsync()
            .Returns(Result.Failure(errorMessage));
        strategy
            .CanBeExecuted(Arg.Is<ProviderTypes>(pt => pt == ProviderTypes.Yandex))
            .Returns(true);
        strategy
            .GetSongFiles(Arg.Is(dataProviderPredicate))
            .Returns(songFiles);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.AddDataProvider(
            ProviderTypes.Yandex,
            userId,
            providerName,
            apiToken,
            refreshToken,
            expiresAt);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .AddAsync(Arg.Is(dataProviderPredicate), false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(1)
            .AddRangeAsync(songFiles, false);
        await unitOfWorkMock
            .Received(1)
            .CommitAsync();
        strategy
            .Received(1)
            .CanBeExecuted(ProviderTypes.Yandex);
        await strategy
            .Received(1)
            .GetSongFiles(Arg.Is(dataProviderPredicate));
        Assert.True(providerResult.IsFailure);
        Assert.Equal(errorMessage, providerResult.Error);
    }

    [Fact]
    public async Task UpdateDataProvider_Should_UpdateSongFiles()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderService = Substitute.For<IExternalProviderService>();
        var strategies = new List<IExternalProviderService>() { externalProviderService };
        var provider = new DataProvider()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderType = ProviderTypes.Yandex,
            SongFiles = new List<SongFile>()
            {
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "1"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "2"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "3"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "4"},
            }
        };
        var updatedSongFiles = new SongFile[]
        {
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "3"},
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "4"},
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "5"},
        };

        unitOfWorkMock
            .DataProviderRepository
            .GetAsync(provider.Id, true, true)
            .Returns(provider);
        unitOfWorkMock
            .SongFileRepository
            .RemoveAsync(Arg.Is<SongFile>(sf => sf.FileId == "1" || sf.FileId == "2"), false)
            .Returns(Result.Success());
        unitOfWorkMock
            .SongFileRepository
            .AddRangeAsync(Arg.Is<SongFile[]>(sf => sf.Length == 1 && sf[0].FileId == "5"))
            .Returns(Result.Success());
        unitOfWorkMock
            .CommitAsync()
            .Returns(Result.Success());
        externalProviderService
            .CanBeExecuted(Arg.Is<ProviderTypes>(pt => pt == ProviderTypes.Yandex))
            .Returns(true);
        externalProviderService
            .GetSongFiles(provider)
            .Returns(updatedSongFiles);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.UpdateDataProvider(provider.Id, provider.UserId);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .GetAsync(provider.Id, true, true);
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .UpdateAsync(provider, false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(2)
            .RemoveAsync(Arg.Is<SongFile>(sf => sf.FileId == "1" || sf.FileId == "2"), false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(1)
            .AddRangeAsync(Arg.Is<IEnumerable<SongFile>>(sf => sf.Single().FileId == "5"), false);
        await unitOfWorkMock
            .Received(1)
            .CommitAsync();
        externalProviderService
            .Received(1)
            .CanBeExecuted(ProviderTypes.Yandex);
        await externalProviderService
            .Received(1)
            .GetSongFiles(provider);
        Assert.True(providerResult.IsSuccess);
    }

    [Fact]
    public async Task UpdateDataProvider_Should_ReturnFailure_When_UnitOfWorkFailsToCommit()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderService = Substitute.For<IExternalProviderService>();
        var strategies = new List<IExternalProviderService>() { externalProviderService };
        var provider = new DataProvider()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            ProviderType = ProviderTypes.Yandex,
            SongFiles = new List<SongFile>()
            {
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "1"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "2"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "3"},
                new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "4"},
            }
        };
        var updatedSongFiles = new SongFile[]
        {
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "3"},
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "4"},
            new () { Id = Guid.NewGuid(), Hash = "", Name = "", Path = "", Type = AudioTypes.Mp3, FileId = "5"},
        };
        var errorMessage = "To Infinity and Beyond!";

        unitOfWorkMock
            .DataProviderRepository
            .GetAsync(provider.Id, true, true)
            .Returns(provider);
        unitOfWorkMock
            .SongFileRepository
            .RemoveAsync(Arg.Is<SongFile>(sf => sf.FileId == "1" || sf.FileId == "2"), false)
            .Returns(Result.Success());
        unitOfWorkMock
            .SongFileRepository
            .AddRangeAsync(Arg.Is<SongFile[]>(sf => sf.Length == 1 && sf[0].FileId == "5"))
            .Returns(Result.Success());
        unitOfWorkMock
            .CommitAsync()
            .Returns(Result.Failure(errorMessage));
        externalProviderService
            .CanBeExecuted(Arg.Is<ProviderTypes>(pt => pt == ProviderTypes.Yandex))
            .Returns(true);
        externalProviderService
            .GetSongFiles(provider)
            .Returns(updatedSongFiles);

        var service = new ProviderService(unitOfWorkMock, strategies);

        // Act
        var providerResult = await service.UpdateDataProvider(provider.Id, provider.UserId);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .GetAsync(provider.Id, true, true);
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .UpdateAsync(provider, false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(2)
            .RemoveAsync(Arg.Is<SongFile>(sf => sf.FileId == "1" || sf.FileId == "2"), false);
        await unitOfWorkMock
            .SongFileRepository
            .Received(1)
            .AddRangeAsync(Arg.Is<IEnumerable<SongFile>>(sf => sf.Single().FileId == "5"), false);
        await unitOfWorkMock
            .Received(1)
            .CommitAsync();
        externalProviderService
            .Received(1)
            .CanBeExecuted(ProviderTypes.Yandex);
        await externalProviderService
            .Received(1)
            .GetSongFiles(provider);
        Assert.True(providerResult.IsFailure);
        Assert.Equal(errorMessage, providerResult.Error);
    }

    [Fact]
    public async Task UpdateDataProvider_Should_ReturnFailure_When_UserIsNotDataProviderOwner()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderServices = Substitute.For<IEnumerable<IExternalProviderService>>();
        var ownerId = Guid.NewGuid();
        var provider = new DataProvider()
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
        };

        unitOfWorkMock
            .DataProviderRepository
            .GetAsync(provider.Id, true, true)
            .Returns(provider);

        var service = new ProviderService(unitOfWorkMock, externalProviderServices);

        // Act
        var providerResult = await service.UpdateDataProvider(provider.Id, ownerId);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .GetAsync(provider.Id, true, true);
        Assert.True(providerResult.IsFailure);
        Assert.Equal("User is not the owner of the DataProvider", providerResult.Error);
    }
    [Fact]
    public async Task UpdateDataProvider_Should_ReturnFailure_When_ProviderNotExist()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderServices = Substitute.For<IEnumerable<IExternalProviderService>>();
        var userId = Guid.NewGuid();
        var providerId = Guid.NewGuid();

        unitOfWorkMock
            .DataProviderRepository
            .GetAsync(providerId, true, true)
            .ReturnsNull();

        var service = new ProviderService(unitOfWorkMock, externalProviderServices);

        // Act
        var providerResult = await service.UpdateDataProvider(providerId, userId);

        // Assert
        await unitOfWorkMock
            .DataProviderRepository
            .Received(1)
            .GetAsync(providerId, true, true);
        Assert.True(providerResult.IsFailure);
        Assert.Equal("DataProvider was not found", providerResult.Error);
    }

    [Fact]
    public async Task RemoveDataProvider_Should_Remove()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderServices = Substitute.For<IEnumerable<IExternalProviderService>>();
        var provider = new DataProvider { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };

        unitOfWorkMock
            .DataProviderRepository
            .RemoveAsync(provider.Id, provider.UserId, true)
            .Returns(Result.Success());

        var providerService = new ProviderService(unitOfWorkMock, externalProviderServices);

        // Act
        var result = await providerService.RemoveDataProvider(provider.Id, provider.UserId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).RemoveAsync(provider.Id, provider.UserId, true);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task RemoveDataProvider_Should_ReturnFailure_When_RepositoryFailsToRemove()
    {
        // Arrange
        var unitOfWorkMock = Substitute.For<IUnitOfWork>();
        var externalProviderServices = Substitute.For<IEnumerable<IExternalProviderService>>();
        var provider = new DataProvider { Id = Guid.NewGuid(), UserId = Guid.NewGuid() };
        var errorMessage = "To Infinity and Beyond!";

        unitOfWorkMock
            .DataProviderRepository
            .RemoveAsync(provider.Id, provider.UserId, true)
            .Returns(Result.Failure(errorMessage));

        var providerService = new ProviderService(unitOfWorkMock, externalProviderServices);

        // Act
        var result = await providerService.RemoveDataProvider(provider.Id, provider.UserId);

        // Assert
        await unitOfWorkMock.DataProviderRepository.Received(1).RemoveAsync(provider.Id, provider.UserId, true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }
}
