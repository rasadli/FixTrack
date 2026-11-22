using FixTrack.Api.Security;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;

namespace FixTrack.Api.Extensions;

public static class MockAuthExtensions
{
    // Registers the mock bearer scheme and current requester.
    public static IServiceCollection AddMockAuthentication(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentRequester, HttpContextCurrentRequester>();
        services.AddAuthentication(FixTrackClaims.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, MockBearerAuthenticationHandler>(
                FixTrackClaims.SchemeName, _ => { });
        services.AddAuthorization();

        return services;
    }
}
