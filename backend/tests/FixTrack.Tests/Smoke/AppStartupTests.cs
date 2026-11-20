using System.Net;
using FixTrack.Tests.Infrastructure;

namespace FixTrack.Tests.Smoke;

public sealed class AppStartupTests(FixTrackApiFactory factory) : IClassFixture<FixTrackApiFactory>
{
    [Fact]
    public async Task UnknownRoute_Returns404()
    {
        using var client = factory.CreateClientAs(null);

        var response = await client.GetAsync("/api/v1/does-not-exist");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
