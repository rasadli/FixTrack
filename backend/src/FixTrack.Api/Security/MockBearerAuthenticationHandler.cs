using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using FixTrack.Application.Common;
using FixTrack.Application.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace FixTrack.Api.Security;

public sealed class MockBearerAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private const string BearerPrefix = "Bearer ";
    private const string MissingTokenMessage = "Authentication required. Send an 'Authorization: Bearer <token>' header.";
    private const string InvalidTokenMessage = "The bearer token is invalid.";

    private static readonly JsonSerializerOptions ResponseJsonOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(HeaderNames.Authorization, out var authorization))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var header = authorization.ToString();
        if (!header.StartsWith(BearerPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return Task.FromResult(AuthenticateResult.Fail(InvalidTokenMessage));
        }

        var token = header[BearerPrefix.Length..].Trim();
        if (!FixTrackClaims.TokenToUserId.TryGetValue(token, out var userId))
        {
            return Task.FromResult(AuthenticateResult.Fail(InvalidTokenMessage));
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

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        var message = Request.Headers.ContainsKey(HeaderNames.Authorization)
            ? InvalidTokenMessage
            : MissingTokenMessage;

        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json; charset=utf-8";
        await Response.WriteAsync(JsonSerializer.Serialize(ApiErrors.Unauthorized(message), ResponseJsonOptions));
    }
}
