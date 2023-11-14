using System.Security.Claims;
using CloudMusicPlayer.API.Errors;
using CloudMusicPlayer.Core;
using CloudMusicPlayer.Core.Errors;

namespace CloudMusicPlayer.API.Utils;

public static class UserExtensions
{
    public static Result<Guid> GetUserGuid(this ClaimsPrincipal principal)
    {
        if (!Guid.TryParse(principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
        {
            return Result.Failure<Guid>(ApplicationLayerErrors.HttpContext.InvalidUserId());
        }

        return Result.Success(userId);
    }
}
