using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.Core.Models;

public class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset PasswordUpdatedAt { get; set; }
    public History History { get; set; } = null!;

    public static Result<User> Create(string email, string name, string passwordHash)
    {
        if (!Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"))
        {
            return Result.Failure<User>("Email is incorrect");
        }

        if (name.Length is <= 2 or >= 30)
        {
            return Result.Failure<User>("Name must have at least 2 characters and less than 30");
        }

        var currentDate = DateTimeOffset.UtcNow;

        var user = new User
        {
            Name = name,
            Email = email,
            PasswordHash = passwordHash,
            CreatedAt = currentDate,
            PasswordUpdatedAt = currentDate,
            UpdatedAt = currentDate,
            History = new History()
        };

        return Result.Success(user);
    }
}
