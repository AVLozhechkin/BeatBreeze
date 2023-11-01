using System.Linq.Expressions;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Core.Services;
using CloudMusicPlayer.Core.UnitOfWorks;
using CSharpFunctionalExtensions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit;
using Xunit.Sdk;

namespace CloudMusicPlayer.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUser_Should_Create()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var email = "test@test.com";
        var name = "abc";
        var passwordHash = "##########";
        Expression<Predicate<User>> userPredicate = u => u.Email == email &&
                                                         u.Name == name &&
                                                         u.CreatedAt != default &&
                                                         u.UpdatedAt != default &&
                                                         u.PasswordUpdatedAt != default &&
                                                         !string.IsNullOrWhiteSpace(u.PasswordHash);

        userRepositoryMock.AddAsync(Arg.Is(userPredicate), true)
            .Returns(Result.Success());

        var userService = new UserService(userRepositoryMock);

        // Act
        var result = await userService.CreateUser(email, name, passwordHash);

        // Assert
        await userRepositoryMock
            .Received(1)
            .AddAsync(Arg.Is(userPredicate), true);
        Assert.True(result.IsSuccess);
        Assert.Equal(name, result.Value.Name);
        Assert.Equal(email, result.Value.Email);
        Assert.Equal(passwordHash, result.Value.PasswordHash);
    }

    [Theory]
    [InlineData("test@test.com", "ac", "Name must have at least 2 characters and less than 30")]
    [InlineData("test@test.com", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Name must have at least 2 characters and less than 30")]
    [InlineData("testmail", "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Email is incorrect")]
    public async Task CreateUser_Should_ReturnFailure_When_UserCantBeCreated(string email, string name, string expectedErrorMessage)
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var passwordHash = "##########";

        var userService = new UserService(userRepositoryMock);

        // Act
        var result = await userService.CreateUser(email, name, passwordHash);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal(expectedErrorMessage, result.Error);
    }

    [Fact]
    public async Task CreateUser_Should_ReturnFailure_When_RepositoryFailsToAdd()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var email = "test@test.com";
        var name = "abc";
        var passwordHash = "##########";
        Expression<Predicate<User>> userPredicate = u => u.Email == email &&
                                                         u.Name == name &&
                                                         u.CreatedAt != default &&
                                                         u.UpdatedAt != default &&
                                                         u.PasswordUpdatedAt != default &&
                                                         !string.IsNullOrWhiteSpace(u.PasswordHash);
        var errorMessage = "To Infinity and Beyond!";

        userRepositoryMock.AddAsync(Arg.Is(userPredicate), true)
            .Returns(Result.Failure(errorMessage));

        var userService = new UserService(userRepositoryMock);

        // Act
        var result = await userService.CreateUser(email, name, passwordHash);

        // Assert
        await userRepositoryMock
            .Received(1)
            .AddAsync(Arg.Is(userPredicate), true);
        Assert.True(result.IsFailure);
        Assert.Equal(errorMessage, result.Error);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnUser()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var userId = Guid.NewGuid();
        var user = new User { };

        userRepositoryMock.GetByIdAsync(userId, true)
            .Returns(user);

        var userService = new UserService(userRepositoryMock);

        // Act
        var userResult = await userService.GetUserById(userId);

        // Assert
        await userRepositoryMock
            .Received(1)
            .GetByIdAsync(userId, true);
        Assert.True(userResult.IsSuccess);
        Assert.Equal(user, userResult.Value);
    }

    [Fact]
    public async Task GetUserById_Should_ReturnNull_When_RepositoryReturnsNull()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var userId = Guid.NewGuid();

        userRepositoryMock.GetByIdAsync(userId, true)
            .ReturnsNull();

        var userService = new UserService(userRepositoryMock);

        // Act
        var userResult = await userService.GetUserById(userId);

        // Assert
        await userRepositoryMock
            .Received(1)
            .GetByIdAsync(userId, true);
        Assert.True(userResult.IsSuccess);
        Assert.Null(userResult.Value);
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnUser()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var email = "test@test.com";
        var user = new User { };

        userRepositoryMock.GetByEmailAsync(email, true)
            .Returns(user);

        var userService = new UserService(userRepositoryMock);

        // Act
        var userResult = await userService.GetUserByEmail(email);

        // Assert
        await userRepositoryMock
            .Received(1)
            .GetByEmailAsync(email, true);
        Assert.True(userResult.IsSuccess);
        Assert.Equal(user, userResult.Value);
    }

    [Fact]
    public async Task GetUserByEmail_Should_ReturnNull_When_RepositoryReturnsNull()
    {
        // Arrange
        var userRepositoryMock = Substitute.For<IUserRepository>();
        var email = "test@test.com";

        userRepositoryMock.GetByEmailAsync(email, true)
            .ReturnsNull();

        var userService = new UserService(userRepositoryMock);

        // Act
        var userResult = await userService.GetUserByEmail(email);

        // Assert
        await userRepositoryMock
            .Received(1)
            .GetByEmailAsync(email, true);
        Assert.True(userResult.IsSuccess);
        Assert.Null(userResult.Value);
    }
}
