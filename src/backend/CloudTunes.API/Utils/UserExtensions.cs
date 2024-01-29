using System.Security.Claims;

namespace CloudTunes.API.Utils;

public static class UserExtensions
{
    public static Guid GetUserGuid(this ClaimsPrincipal principal)
    {
        return Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}
