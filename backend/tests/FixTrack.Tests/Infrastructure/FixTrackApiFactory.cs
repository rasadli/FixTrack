using System.Net.Http.Headers;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FixTrack.Tests.Infrastructure;

/// <summary>
/// Hosts the FixTrack API in memory for integration tests.
/// </summary>
public sealed class FixTrackApiFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Creates a client that sends <c>Authorization: Bearer &lt;token&gt;</c>,
    /// or no Authorization header when <paramref name="token"/> is <c>null</c>.
    /// </summary>
    public HttpClient CreateClientAs(string? token)
    {
        var client = CreateClient();
        if (token is not null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication();

            // Register TestAuthHandler only if the real "MockBearer" scheme is not registered yet.
            // PostConfigure runs after all application configuration, so it can see whether the
            // real handler (feature/mock-auth-validation) has already added the scheme.
            services.PostConfigure<AuthenticationOptions>(options =>
            {
                if (options.SchemeMap.ContainsKey(FixTrackClaims.SchemeName))
                {
                    return;
                }

                options.AddScheme(FixTrackClaims.SchemeName, scheme => scheme.HandlerType = typeof(TestAuthHandler));
                options.DefaultScheme ??= FixTrackClaims.SchemeName;
            });
        });
    }
}
