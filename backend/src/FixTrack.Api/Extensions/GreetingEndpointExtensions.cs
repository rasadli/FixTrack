using FixTrack.Api.Greeting;

namespace FixTrack.Api.Extensions;

public static class GreetingEndpointExtensions
{
    public static WebApplication MapGreeting(this WebApplication app)
    {
        app.MapGet("/api/v1/greeting", () =>
                Results.Content(GreetingPage.Render(DateTimeOffset.UtcNow), "text/html; charset=utf-8"))
            .AllowAnonymous()
            .ExcludeFromDescription();

        return app;
    }
}
