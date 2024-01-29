using System.Text.RegularExpressions;
using CloudMusicPlayer.Core.Exceptions;

namespace CloudMusicPlayer.Core.Models;

// TODO I guess entity class must not be partial, will refactor it later
public partial record User : BaseEntity
{
    [GeneratedRegex("^((([!#$%&'*+\\-/=?^_`{|}~\\w])|([!#$%&'*+\\-/=?^_`{|}~\\w][!#$%&'*+\\-/=?^_`{|}~\\.\\w]{0,}[!#$%&'*+\\-/=?^_`{|}~\\w]))[@]\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*)$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();

    public User() {}

    public User(string email, string passwordHash)
    {
        if (!EmailRegex().IsMatch(email))
        {
            throw ValidationException.Create(nameof(email), "Email is invalid");
        }

        var currentDate = DateTimeOffset.UtcNow;

        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = currentDate;
        PasswordUpdatedAt = currentDate;
        UpdatedAt = currentDate;
    }

    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset PasswordUpdatedAt { get; set; }
    public Guid LastPlayedMusicFileId { get; set; }
}
