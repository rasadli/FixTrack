using System.Security.Claims;
using FixTrack.Application.Security;

namespace FixTrack.Api.Security;

public sealed class HttpContextCurrentRequester(IHttpContextAccessor httpContextAccessor) : ICurrentRequester
{
    public Requester Get()
    {
        var user = httpContextAccessor.HttpContext?.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new InvalidOperationException("The current request is not authenticated.");
        }

        var userId = user.FindFirstValue(FixTrackClaims.UserId);
        if (string.IsNullOrEmpty(userId))
        {
            throw new InvalidOperationException($"The authenticated user has no '{FixTrackClaims.UserId}' claim.");
        }

        return new Requester(userId, user.IsInRole(FixTrackClaims.AdminRole));
    }
}
