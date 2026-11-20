using FixTrack.Api.Security;
using FixTrack.Application.Security;

namespace FixTrack.Api.Extensions;

public static class MockAuthExtensions
{
    // OWNER: Khavar - implement in feature/mock-auth-validation
    // (register the "MockBearer" authentication scheme and its handler; see FixTrackClaims).
    public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentRequester, HttpContextCurrentRequester>();
        services.AddAuthorization();

        return services;
    }
}
