using System.ComponentModel.DataAnnotations;

namespace BeatBreeze.API.Dtos.Requests;

public record LoginRequest
{
    [RegularExpression(RegexPatterns.EmailPattern)]
    public required string Email { get; set; }

    [RegularExpression(RegexPatterns.PasswordPattern)]
    public required string Password { get; set; }
}
