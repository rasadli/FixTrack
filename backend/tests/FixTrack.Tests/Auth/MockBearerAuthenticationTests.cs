using System.Net;
using System.Text.Json;
using FixTrack.Api.Security;
using FixTrack.Application.Security;
using FixTrack.Tests.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace FixTrack.Tests.Auth;

public sealed class MockBearerAuthenticationTests(FixTrackApiFactory factory) : IClassFixture<FixTrackApiFactory>
{
    private const string CasesUrl = "/api/v1/cases";
    private const string MissingTokenMessage = "Authentication required. Send an 'Authorization: Bearer <token>' header.";
    private const string InvalidTokenMessage = "The bearer token is invalid.";

    [Fact]
    public async Task MockBearerScheme_UsesRealHandler()
    {
        var provider = factory.Services.GetRequiredService<IAuthenticationSchemeProvider>();

        var scheme = await provider.GetSchemeAsync(FixTrackClaims.SchemeName);

        Assert.NotNull(scheme);
        Assert.Equal(typeof(MockBearerAuthenticationHandler), scheme.HandlerType);
    }

    [Theory]
    [InlineData(null, MissingTokenMessage)]
    [InlineData("Bearer nope", InvalidTokenMessage)]
    [InlineData("Basic abc", InvalidTokenMessage)]
    [InlineData("Bearer", InvalidTokenMessage)]
    [InlineData("Bearer ", InvalidTokenMessage)]
    public async Task MissingOrInvalidAuthorization_ReturnsJson401(string? authorization, string expectedMessage)
    {
        using var client = factory.CreateClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, CasesUrl);
        if (authorization is not null)
        {
            request.Headers.TryAddWithoutValidation("Authorization", authorization);
        }

        using var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType?.CharSet);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var body = document.RootElement;
        Assert.False(body.GetProperty("success").GetBoolean());
        var error = body.GetProperty("error");
        Assert.Equal("UNAUTHORIZED", error.GetProperty("code").GetString());
        Assert.Equal(expectedMessage, error.GetProperty("message").GetString());
        Assert.False(error.TryGetProperty("details", out _));
    }

    [Theory]
    [InlineData(FixTrackClaims.UserAToken, 2)]
    [InlineData(FixTrackClaims.UserBToken, 2)]
    [InlineData(FixTrackClaims.AdminToken, 4)]
    public async Task KnownToken_CanListVisibleCases(string token, int expectedCount)
    {
        using var client = factory.CreateClientAs(token);

        using var response = await client.GetAsync(CasesUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal(expectedCount, document.RootElement.GetProperty("data").GetArrayLength());
    }
}
