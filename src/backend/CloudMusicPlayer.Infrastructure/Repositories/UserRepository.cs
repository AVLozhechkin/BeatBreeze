using CloudMusicPlayer.Core.Models;
using CloudMusicPlayer.Core.Repositories;
using CloudMusicPlayer.Infrastructure.Database;
using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;

namespace CloudMusicPlayer.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationContext _applicationContext;

    public UserRepository(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var toBeDeleted = _applicationContext
            .Users
            .Where(u => u.Id == userId);

        return await _applicationContext.ExecuteDeleteResult(toBeDeleted);
    }


    public async Task<User?> GetByNameAsync(string name, bool asNoTracking = true)
    {
        var query = _applicationContext.Users.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Name == name);
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

    public async Task<Result> AddAsync(User user, bool saveChanges = false)
    {
        if (saveChanges)
        {
            await _applicationContext.Users.AddAsync(user);

            return await _applicationContext.SaveChangesResult("User was not added");
        }

        await _applicationContext.Users.AddAsync(user);

        return Result.Success();
    }

    public async Task<Result> UpdateAsync(User user, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeUpdated = _applicationContext.Users.Where(u => u.Id == user.Id);

            return await _applicationContext.ExecuteUpdateResult(toBeUpdated, setters =>
                    setters.SetProperty(u => u.Name, user.Name)
                        .SetProperty(u => u.Email, user.Email)
                        .SetProperty(u => u.PasswordHash, user.PasswordHash)
                        .SetProperty(u => u.CreatedAt, user.CreatedAt)
                        .SetProperty(u => u.UpdatedAt, user.UpdatedAt)
                        .SetProperty(u => u.PasswordUpdatedAt, user.PasswordUpdatedAt),
                "User was not updated");
        }

        _applicationContext.Users.Update(user);

        return Result.Success();
    }

    public async Task<Result> RemoveAsync(Guid userId, bool saveChanges = false)
    {
        if (saveChanges)
        {
            var toBeDeleted = _applicationContext.Users
                .Where(u => u.Id == userId).Take(1);

            return await _applicationContext.ExecuteDeleteResult(toBeDeleted, "User was not deleted");
        }

        var user = new User { Id = userId };

        _applicationContext.Users.Remove(user);

        return Result.Success();
    }
}
