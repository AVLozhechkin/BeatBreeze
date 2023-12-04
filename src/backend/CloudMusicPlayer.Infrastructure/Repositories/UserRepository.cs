using CloudMusicPlayer.Core.Exceptions;
using CloudMusicPlayer.Core.Interfaces.Repositories;
using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationContext _applicationContext;

    public UserRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<User?> GetByEmailAsync(string email, bool asNoTracking = true)
    {
        var query = _applicationContext.Users.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid userId, bool asNoTracking = true)
    {
        var query = _applicationContext.Users.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddAsync(User user, bool saveChanges = false)
    {
        await _applicationContext.Users.AddAsync(user);

        if (saveChanges)
        {
            await _applicationContext.SaveChangesAsync();
        }
    }

    public async Task UpdateAsync(User user, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync( setters =>
                    setters
                        .SetProperty(u => u.Email, user.Email)
                        .SetProperty(u => u.PasswordHash, user.PasswordHash)
                        .SetProperty(u => u.CreatedAt, user.CreatedAt)
                        .SetProperty(u => u.UpdatedAt, user.UpdatedAt)
                        .SetProperty(u => u.PasswordUpdatedAt, user.PasswordUpdatedAt));

            if (result == 0)
            {
                throw NotFoundException.Create<User>(user.Id);
            }

            return;
        }

        _applicationContext.Users.Update(user);
    }

    public async Task RemoveAsync(Guid userId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Users
                .Where(u => u.Id == userId)
                .Take(1)
                .ExecuteDeleteAsync();

            if (result == 0)
            {
                throw NotFoundException.Create<User>(userId);
            }

            return;
        }

        var user = new User { Id = userId };

        _applicationContext.Users.Remove(user);
    }
}
