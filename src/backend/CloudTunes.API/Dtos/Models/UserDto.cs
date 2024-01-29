using CloudTunes.Core.Models;

namespace CloudTunes.API.Dtos.Models;

public record UserDto
{
    public string Email { get; private set; } = null!;

    public static UserDto Create(User user)
    {
        ArgumentNullException.ThrowIfNull(user);

        return new UserDto()
        {
            Email = user.Email
        };
    }
}
