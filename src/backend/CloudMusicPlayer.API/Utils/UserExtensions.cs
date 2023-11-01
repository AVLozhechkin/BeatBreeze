using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace CloudMusicPlayer.API.Utils;

public static class UserExtensions
{
    public static Result<Guid> GetUserGuid(this ClaimsPrincipal principal)
    {
        if (!Guid.TryParse(principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Result.Failure<Guid>("UserId must be a parseable GUID");
        }

        return Result.Success(userId);
    }
}
