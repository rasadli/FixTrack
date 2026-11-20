using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using FixTrack.Application.Cases;
using FixTrack.Application.Cases.Contracts;
using FixTrack.Application.Security;
using FixTrack.Tests.Infrastructure;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FixTrack.Tests.Errors;

public sealed class ErrorEnvelopeTests(FixTrackApiFactory factory) : IClassFixture<FixTrackApiFactory>
{
    [Fact]
    public async Task UnknownRoute_ReturnsResourceNotFound()
    {
        using var client = factory.CreateClient();
        using var response = await client.GetAsync("/api/v1/does-not-exist");

        await AssertError(response, HttpStatusCode.NotFound, "NOT_FOUND", "The requested resource does not exist.");
    }

    [Fact]
    public async Task WrongMethod_ReturnsMethodNotAllowed()
    {
        using var client = factory.CreateClientAs(FixTrackClaims.UserAToken);
        using var request = new HttpRequestMessage(HttpMethod.Patch, "/api/v1/cases/case-101");
        using var response = await client.SendAsync(request);

        await AssertError(response, HttpStatusCode.MethodNotAllowed, "METHOD_NOT_ALLOWED", "The HTTP method is not allowed for this resource.");
    }

    [Fact]
    public async Task NonJsonBody_ReturnsUnsupportedMediaType()
    {
        using var client = factory.CreateClientAs(FixTrackClaims.UserAToken);
        using var body = new StringContent("not json", Encoding.UTF8, "text/plain");
        using var response = await client.PostAsync("/api/v1/cases", body);

        await AssertError(response, HttpStatusCode.UnsupportedMediaType, "UNSUPPORTED_MEDIA_TYPE", "The request body must be JSON (Content-Type: application/json).");
    }

    [Fact]
    public async Task ExistingCase404_KeepsControllerMessage()
    {
        using var client = factory.CreateClientAs(FixTrackClaims.AdminToken);
        using var response = await client.GetAsync("/api/v1/cases/case-999");

        await AssertError(response, HttpStatusCode.NotFound, "NOT_FOUND", "Case not found.");
    }

    [Fact]
    public async Task UnhandledException_ReturnsInternalErrorWithoutDetails()
    {
        using var testFactory = factory.WithWebHostBuilder(builder => builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<ICaseService>();
            services.AddScoped<ICaseService>(_ => new ThrowingCaseService());
        }));
        using var client = testFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", FixTrackClaims.AdminToken);

        using var response = await client.GetAsync("/api/v1/cases");

        Assert.DoesNotContain("secret-detail", await response.Content.ReadAsStringAsync());
        await AssertError(response, HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.");
    }

    private static async Task AssertError(HttpResponseMessage response, HttpStatusCode status, string code, string message)
    {
        Assert.Equal(status, response.StatusCode);
        Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("utf-8", response.Content.Headers.ContentType?.CharSet);

        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;
        Assert.False(root.GetProperty("success").GetBoolean());
        var error = root.GetProperty("error");
        Assert.Equal(code, error.GetProperty("code").GetString());
        Assert.Equal(message, error.GetProperty("message").GetString());
        Assert.False(error.TryGetProperty("details", out _));
    }

    private sealed class ThrowingCaseService : ICaseService
    {
        public CasePage List(int page, int perPage) => throw new InvalidOperationException("secret-detail");
        public CaseResponse? Get(string id) => throw new NotSupportedException();
        public CaseResponse Create(CreateCaseRequest request) => throw new NotSupportedException();
        public CaseResponse? Update(string id, UpdateCaseRequest request) => throw new NotSupportedException();
        public bool Delete(string id) => throw new NotSupportedException();
    }
}
