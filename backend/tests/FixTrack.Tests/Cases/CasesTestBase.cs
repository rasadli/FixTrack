using System.Net.Http.Json;
using System.Text.Json;
using FixTrack.Tests.Infrastructure;

namespace FixTrack.Tests.Cases;

public abstract class CasesTestBase : IDisposable
{
    protected const string CasesUrl = "/api/v1/cases";

    protected FixTrackApiFactory Factory { get; } = new();

    protected HttpClient ClientAs(string? token) => Factory.CreateClientAs(token);

    protected static async Task<JsonElement> ReadJsonAsync(HttpResponseMessage response)
    {
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.Clone();
    }

    protected static JsonContent Json(object body) => JsonContent.Create(body);

    protected static object ValidUpdate(string status = "InProgress") => new
    {
        customerName = "Updated Customer",
        device = "Updated Device",
        issueDescription = "Updated description of the reported issue.",
        caseType = "Warranty",
        status
    };

    public void Dispose()
    {
        Factory.Dispose();
        GC.SuppressFinalize(this);
    }
}
