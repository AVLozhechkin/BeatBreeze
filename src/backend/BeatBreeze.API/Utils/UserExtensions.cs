using System.Security.Claims;

namespace BeatBreeze.API.Utils;

public static class UserExtensions
{
    public static Guid GetUserGuid(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
