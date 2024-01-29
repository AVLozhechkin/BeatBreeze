using System.ComponentModel.DataAnnotations;

namespace CloudTunes.API.Dtos.Requests;

public record CreateUserRequest
{
    [RegularExpression(RegexPatterns.EmailPattern)]
    public required string Email { get; set; }

    [RegularExpression(RegexPatterns.PasswordPattern)]
    public required string Password { get; set; }

    [RegularExpression(RegexPatterns.PasswordPattern)]
    public required string PasswordConfirmation { get; set; }
}
