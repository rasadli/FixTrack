using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FixTrack.Application.Security;
using FixTrack.Tests.Infrastructure;

namespace FixTrack.Tests.Validation;

public sealed class CaseValidationTests(FixTrackApiFactory factory) : IClassFixture<FixTrackApiFactory>
{
    private const string CasesUrl = "/api/v1/cases";

    [Theory]
    [InlineData(false, "customerName")]
    [InlineData(false, "device")]
    [InlineData(false, "issueDescription")]
    [InlineData(false, "caseType")]
    [InlineData(true, "customerName")]
    [InlineData(true, "device")]
    [InlineData(true, "issueDescription")]
    [InlineData(true, "caseType")]
    [InlineData(true, "status")]
    public async Task MissingOrNullField_ReturnsRequiredError(bool update, string field)
    {
        foreach (var omit in new[] { true, false })
        {
            var body = ValidBody(update);
            if (omit) body.Remove(field);
            else body[field] = null;

            var result = await SendJson(update, body);
            AssertValidation(result, field);
            Assert.Equal($"{field} is required.", ErrorMessage(result.Body, field));
        }
    }

    [Theory]
    [InlineData("customerName")]
    [InlineData("device")]
    [InlineData("issueDescription")]
    public async Task WhitespaceOnlyText_IsMissing(string field)
    {
        foreach (var update in new[] { false, true })
        {
            var body = ValidBody(update);
            body[field] = "   ";

            var result = await SendJson(update, body);
            AssertValidation(result, field);
            Assert.Equal($"{field} is required.", ErrorMessage(result.Body, field));
        }
    }

    [Theory]
    [InlineData("customerName", 1, false)]
    [InlineData("customerName", 2, true)]
    [InlineData("customerName", 100, true)]
    [InlineData("customerName", 101, false)]
    [InlineData("device", 1, false)]
    [InlineData("device", 2, true)]
    [InlineData("device", 100, true)]
    [InlineData("device", 101, false)]
    [InlineData("issueDescription", 9, false)]
    [InlineData("issueDescription", 10, true)]
    [InlineData("issueDescription", 1000, true)]
    [InlineData("issueDescription", 1001, false)]
    public async Task LengthBoundaries_UseTrimmedValue(string field, int length, bool valid)
    {
        foreach (var update in new[] { false, true })
        {
            var body = ValidBody(update);
            body[field] = $" {new string('x', length)} ";

            var result = await SendJson(update, body);
            if (valid)
            {
                Assert.Equal(update ? HttpStatusCode.OK : HttpStatusCode.Created, result.Status);
                Assert.Equal(new string('x', length), result.Body.GetProperty("data").GetProperty(field).GetString());
            }
            else
            {
                AssertValidation(result, field);
            }
        }
    }

    [Theory]
    [InlineData(false, "caseType", "Other")]
    [InlineData(true, "caseType", "Other")]
    [InlineData(true, "status", "Other")]
    [InlineData(false, "caseType", 0)]
    [InlineData(true, "status", 0)]
    [InlineData(false, "customerName", 123)]
    [InlineData(true, "customerName", 123)]
    public async Task InvalidEnumsAndTypes_Return400(bool update, string field, object value)
    {
        var body = ValidBody(update);
        body[field] = value;

        AssertValidation(await SendJson(update, body), field);
    }

    [Theory]
    [InlineData("ownerId")]
    [InlineData("id")]
    [InlineData("trackingCode")]
    [InlineData("createdAt")]
    [InlineData("updatedAt")]
    [InlineData("foo")]
    public async Task UnknownField_Returns400(string field)
    {
        foreach (var update in new[] { false, true })
        {
            var body = ValidBody(update);
            body[field] = "not allowed";

            AssertValidation(await SendJson(update, body), field);
        }
    }

    [Fact]
    public async Task Post_StatusIsUnknownField()
    {
        var body = ValidBody(false);
        body["status"] = "Completed";

        AssertValidation(await SendJson(false, body), "status");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MalformedOrEmptyBody_Returns400(bool update)
    {
        AssertValidation(await Send(update, new StringContent("{", Encoding.UTF8, "application/json")), "body");
        AssertValidation(await Send(update, new StringContent("", Encoding.UTF8, "application/json")), "body");
        AssertValidation(await Send(update, null), "body");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MultipleErrors_AreReportedTogether(bool update)
    {
        var body = ValidBody(update);
        body["customerName"] = " ";
        body["device"] = "x";
        body["issueDescription"] = "short";
        body.Remove("caseType");
        if (update) body.Remove("status");

        var fields = update
            ? new[] { "customerName", "device", "issueDescription", "caseType", "status" }
            : new[] { "customerName", "device", "issueDescription", "caseType" };
        AssertValidation(await SendJson(update, body), fields);
    }

    [Theory]
    [InlineData(false, HttpStatusCode.Created)]
    [InlineData(true, HttpStatusCode.OK)]
    public async Task ValidBody_SucceedsAndStoresTrimmedText(bool update, HttpStatusCode status)
    {
        var body = ValidBody(update);
        body["customerName"] = "  Jane Doe  ";

        var result = await SendJson(update, body);

        Assert.Equal(status, result.Status);
        Assert.Equal("Jane Doe", result.Body.GetProperty("data").GetProperty("customerName").GetString());
    }

    private static Dictionary<string, object?> ValidBody(bool update)
    {
        var body = new Dictionary<string, object?>
        {
            ["customerName"] = "Jane Doe",
            ["device"] = "Laptop",
            ["issueDescription"] = "The device does not turn on.",
            ["caseType"] = "Repair"
        };
        if (update) body["status"] = "Open";
        return body;
    }

    private Task<(HttpStatusCode Status, JsonElement Body)> SendJson(bool update, object body) =>
        Send(update, JsonContent.Create(body));

    private async Task<(HttpStatusCode Status, JsonElement Body)> Send(bool update, HttpContent? content)
    {
        using var requestContent = content;
        using var client = factory.CreateClientAs(FixTrackClaims.UserAToken);
        using var response = update
            ? await client.PutAsync($"{CasesUrl}/case-101", content)
            : await client.PostAsync(CasesUrl, content);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return (response.StatusCode, json.RootElement.Clone());
    }

    private static void AssertValidation((HttpStatusCode Status, JsonElement Body) result, params string[] fields)
    {
        Assert.Equal(HttpStatusCode.BadRequest, result.Status);
        Assert.False(result.Body.GetProperty("success").GetBoolean());
        var error = result.Body.GetProperty("error");
        Assert.Equal("VALIDATION_ERROR", error.GetProperty("code").GetString());
        Assert.Equal("Invalid request.", error.GetProperty("message").GetString());
        var actual = error.GetProperty("details").EnumerateArray()
            .Select(item => item.GetProperty("field").GetString()).ToArray();
        Assert.Equal(fields.Length, actual.Length);
        foreach (var field in fields) Assert.Contains(field, actual);
    }

    private static string? ErrorMessage(JsonElement body, string field) =>
        body.GetProperty("error").GetProperty("details").EnumerateArray()
            .Single(item => item.GetProperty("field").GetString() == field)
            .GetProperty("message").GetString();
}
