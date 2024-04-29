using BeatBreeze.Core.Models;

namespace BeatBreeze.Core.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(string email, string passwordHash);
    Task<User?> GetUserById(Guid userId);
    Task<User?> GetUserByEmail(string email);
}
