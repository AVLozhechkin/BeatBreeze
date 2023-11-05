using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Services;

public sealed class UserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<User>> CreateUser(string email, string name, string passwordHash)
    {
        var userCreationResult = User.Create(email, name, passwordHash);

        if (userCreationResult.IsFailure)
        {
            return Result.Failure<User>(userCreationResult.Error);
        }

        var result = await _userRepository.AddAsync(userCreationResult.Value, true);

        if (result.IsFailure)
        {
            return Result.Failure<User>(result.Error);
        }

        return Result.Success(userCreationResult.Value);

    }

    public async Task<Result<User?>> GetUserById(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        return Result.Success(user);
    }

    public async Task<Result<User?>> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        return Result.Success(user);
    }
}
