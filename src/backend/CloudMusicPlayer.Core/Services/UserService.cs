using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using Microsoft.Extensions.Logging;

namespace CloudMusicPlayer.Core.Services;

internal sealed class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    public UserService(ILogger<UserService> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }

    public async Task<User> CreateUser(string email, string passwordHash)
    {
        var user = new User(email, passwordHash);

        await _userRepository.AddAsync(user, true);
        _logger.LogInformation("User ({UserId}) was created", user.Id);

        return user;

    }

    public async Task<User?> GetUserById(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId, true);

        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email, true);

        return user;
    }
}
