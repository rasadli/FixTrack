using System.Security.Claims;
using System.Text.Encodings.Web;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace FixTrack.Tests.Infrastructure;

// Test stand-in for the real MockBearer handler; accepts the same tokens and issues the same claims.
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
