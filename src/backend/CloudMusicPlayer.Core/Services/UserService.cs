using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;

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
        var userResult = await _userRepository.GetByIdAsync(userId);

        return userResult;
    }

    public async Task<Result<User?>> GetUserByEmail(string email)
    {
        var userResult = await _userRepository.GetByEmailAsync(email);

        return userResult;
    }
}
