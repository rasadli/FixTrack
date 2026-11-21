using System.Net;
using FixTrack.Tests.Infrastructure;

namespace FixTrack.Tests.Greeting;

public sealed class GreetingEndpointTests(FixTrackApiFactory factory) : IClassFixture<FixTrackApiFactory>
{
    [Theory]
    [InlineData(null)]
    [InlineData("nope")]
    public async Task Greeting_IsAccessibleWithoutAValidToken(string? token)
    {
        using var client = factory.CreateClientAs(token);
        using var response = await client.GetAsync("/api/v1/greeting");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType?.ToString());

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("FixTrack", html);
        Assert.Contains("Rashad Asadli", html);
        Assert.Contains("Khavar Jabbarli", html);
        Assert.Contains("Faig Almuradli", html);
        Assert.Contains("Student ID: 17960", html);
        Assert.Contains("Student ID: 18167", html);
        Assert.Contains("Student ID: 23168", html);
        Assert.Contains("<code>/api/v1/cases</code>", html);
        Assert.Contains("<code>/api/v1/cases/{id}</code>", html);
        Assert.Contains("<code>/api/v1/greeting</code>", html);
        Assert.Contains("Running", html);
        Assert.Contains("API v1", html);
        Assert.Contains("""<meta name="viewport" content="width=device-width, initial-scale=1">""", html);
    }
}
