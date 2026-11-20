using System.Security.Claims;
using System.Text.Encodings.Web;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixTrack.Tests.Infrastructure;

/// <summary>
/// Test-only stand-in for the real mock bearer authentication handler.
/// </summary>
/// <remarks>
/// The real handler is implemented on feature/mock-auth-validation. Until it is merged,
/// <see cref="FixTrackApiFactory"/> registers this handler under the same scheme name so that
/// other branches (for example feature/cases-core) can already write authenticated tests.
/// Once the real "MockBearer" scheme exists, the factory skips this handler and tests run
/// against the real one. It recognises the same tokens and produces the same claims as
/// <see cref="FixTrackClaims"/> describes.
/// </remarks>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string BearerPrefix = "Bearer ";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        string? header = Request.Headers.Authorization;

        if (string.IsNullOrEmpty(header))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail("Unsupported authorization scheme."));
        }

        var token = header[BearerPrefix.Length..].Trim();
        if (!FixTrackClaims.TokenToUserId.TryGetValue(token, out var userId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid token."));
        }

        var claims = new List<Claim> { new(FixTrackClaims.UserId, userId) };
        if (FixTrackClaims.IsAdminUser(userId))
        {
            claims.Add(new Claim(ClaimTypes.Role, FixTrackClaims.AdminRole));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
