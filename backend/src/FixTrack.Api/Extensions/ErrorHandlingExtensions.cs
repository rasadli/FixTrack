namespace FixTrack.Api.Extensions;

public static class ErrorHandlingExtensions
{
    // OWNER: Khavar - implement in feature/mock-auth-validation
    // (global exception handler returning ApiErrors.Internal(), envelopes for 401/404/405/415).
    public static WebApplication UseFixTrackErrorHandling(this WebApplication app)
    {
        return app;
    }
}
