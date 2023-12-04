using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record UserDto
{
    public string? Email { get; set; }

    public static UserDto Create(User user)
    {
        var dto = new UserDto()
        {
            Email = user.Email
        };

        return dto;
    }
}
