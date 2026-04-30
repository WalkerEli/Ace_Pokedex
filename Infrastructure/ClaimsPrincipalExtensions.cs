using System.Security.Claims;

namespace TeamAceProject.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetCurrentUserId(this ClaimsPrincipal principal)
    {
        string? value = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out Guid userId) ? userId : null;
    }
}
