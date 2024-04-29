using BeatBreeze.Core.Exceptions;
using BeatBreeze.Core.Interfaces.Repositories;
using BeatBreeze.Core.Models;
using BeatBreeze.Infrastructure.Database;
using BeatBreeze.Infrastructure.Errors;
using Microsoft.EntityFrameworkCore;

namespace BeatBreeze.Infrastructure.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ApplicationContext _applicationContext;
    private readonly IExceptionParser _exceptionParser;

    public UserRepository(ApplicationContext applicationContext, IExceptionParser exceptionParser)
    {
        _applicationContext = applicationContext;
        _exceptionParser = exceptionParser;
    }

    public async Task<User?> GetByEmailAsync(string email, bool asNoTracking)
    {
        var query = _applicationContext.Users.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(Guid userId, bool asNoTracking)
    {
        var query = _applicationContext.Users.AsQueryable();

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        return await query.FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task AddAsync(User user, bool saveChanges)
    {
        await _applicationContext.Users.AddAsync(user);

        if (saveChanges)
        {
            try
            {
                await _applicationContext.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (_exceptionParser.IsAlreadyExists(ex))
            {
                throw AlreadyExistException.Create<User>();
            }
        }
    }

    public async Task UpdateAsync(User user, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Users
                .Where(u => u.Id == user.Id)
                .ExecuteUpdateAsync(setters =>
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

    public async Task RemoveAsync(Guid userId, bool saveChanges)
    {
        if (saveChanges)
        {
            var result = await _applicationContext
                .Users
                .Where(u => u.Id == userId)
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
