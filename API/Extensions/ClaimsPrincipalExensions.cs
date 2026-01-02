using System;
using System.Security.Claims;

namespace API.Extensions;

public static class ClaimsPrincipalExensions
{
    public static int GetUserId(this ClaimsPrincipal User)
    {
        return int.TryParse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out int userId) ? userId : 0;
    }

    public static string GetUsername(this ClaimsPrincipal User)
    {
        return User.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string GetEmail(this ClaimsPrincipal User)
    {
        return User.FindFirst(ClaimTypes.Email)?.Value;
    }
}
