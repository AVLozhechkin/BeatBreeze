using CloudMusicPlayer.Core.Models;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByNameAsync(string name, bool asNoTracking = true);
    public Task<User?> GetByEmailAsync(string email, bool asNoTracking = true);
    public Task<User?> GetByIdAsync(Guid userId, bool asNoTracking = true);
    public Task<Result> AddAsync(User user, bool saveChanges = false);
    public Task<Result> UpdateAsync(User user, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid userId, bool saveChanges = false);
}
