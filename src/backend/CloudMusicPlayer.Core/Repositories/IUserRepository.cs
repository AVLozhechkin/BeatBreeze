using CloudMusicPlayer.Core.Errors;
using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Repositories;

public interface IUserRepository
{
    public Task<Result<User?>> GetByNameAsync(string name, bool asNoTracking = true);
    public Task<Result<User?>> GetByEmailAsync(string email, bool asNoTracking = true);
    public Task<Result<User?>> GetByIdAsync(Guid userId, bool asNoTracking = true);
    public Task<Result> AddAsync(User user, bool saveChanges = false);
    public Task<Result> UpdateAsync(User user, bool saveChanges = false);
    public Task<Result> RemoveAsync(Guid userId, bool saveChanges = false);
}
