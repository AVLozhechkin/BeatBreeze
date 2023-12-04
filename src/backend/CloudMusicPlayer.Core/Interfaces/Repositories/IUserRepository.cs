using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.Core.Interfaces.Repositories;

public interface IUserRepository
{
    public Task<User?> GetByEmailAsync(string email, bool asNoTracking);
    public Task<User?> GetByIdAsync(Guid userId, bool asNoTracking);
    public Task AddAsync(User user, bool saveChanges);
    public Task UpdateAsync(User user, bool saveChanges);
    public Task RemoveAsync(Guid userId, bool saveChanges);
}
