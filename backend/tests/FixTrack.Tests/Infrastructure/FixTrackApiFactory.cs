using System.Net.Http.Headers;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace FixTrack.Tests.Infrastructure;

public sealed class FixTrackApiFactory : WebApplicationFactory<Program>
{
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

            // Fall back to TestAuthHandler only when the app has no real MockBearer scheme.
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
