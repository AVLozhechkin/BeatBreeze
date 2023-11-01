using CloudMusicPlayer.Core.Models;

namespace CloudMusicPlayer.API.Dtos.Models;

public record UserDto
{
    public string? Name { get; set; }
    public string? Email { get; set; }

    public static UserDto Create(User user)
    {
        var dto = new UserDto()
        {
            Name = user.Name,
            Email = user.Email
        };

        return dto;
    }
}
