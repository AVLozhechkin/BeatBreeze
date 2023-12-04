using System.Text.RegularExpressions;
using CloudMusicPlayer.Core.Exceptions;

namespace CloudMusicPlayer.Core.Models;

public record User : BaseEntity
{
    public User() {}

    public User(string email, string passwordHash)
    {
        if (!Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21\\x23-\\x5b\\x5d-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\\x01-\\x08\\x0b\\x0c\\x0e-\\x1f\\x21-\\x5a\\x53-\\x7f]|\\\\[\\x01-\\x09\\x0b\\x0c\\x0e-\\x7f])+)\\])"))
        {
            throw new ValidationException("Email is invalid");
        }

        var currentDate = DateTimeOffset.UtcNow;

        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = currentDate;
        PasswordUpdatedAt = currentDate;
        UpdatedAt = currentDate;
        History = new History();
    }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset PasswordUpdatedAt { get; set; }
    public History History { get; set; } = null!;
}
