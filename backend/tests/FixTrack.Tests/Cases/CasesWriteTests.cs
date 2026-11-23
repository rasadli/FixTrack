using System.Net;
using System.Text.Json;
using FixTrack.Application.Security;

namespace FixTrack.Tests.Cases;

public sealed class CasesWriteTests : CasesTestBase
{
    private static readonly object ValidCreate = new
    {
        customerName = "  New Customer  ",
        device = "Phone",
        issueDescription = "Battery drains within two hours of a full charge.",
        caseType = "Repair"
    };

    [Fact]
    public async Task Create_Returns201_WithLocation_OwnedByRequester()
    {
        var response = await ClientAs(FixTrackClaims.UserBToken).PostAsync(CasesUrl, Json(ValidCreate));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/v1/cases/case-105", response.Headers.Location?.AbsolutePath);

        var json = await ReadJsonAsync(response);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal("Case created.", json.GetProperty("message").GetString());
        var data = json.GetProperty("data");
        Assert.Equal("case-105", data.GetProperty("id").GetString());
        Assert.Matches("^FT-[A-Z0-9]{4}$", data.GetProperty("trackingCode").GetString());
        Assert.Equal("user-b", data.GetProperty("ownerId").GetString());
        Assert.Equal("New Customer", data.GetProperty("customerName").GetString());
        Assert.Equal("Open", data.GetProperty("status").GetString());
        Assert.False(data.TryGetProperty("updatedAt", out _));

        var fetched = await ClientAs(FixTrackClaims.UserBToken).GetAsync($"{CasesUrl}/case-105");
        Assert.Equal(HttpStatusCode.OK, fetched.StatusCode);
    }

    [Fact]
    public async Task Create_CannotSetOwnerThroughBody()
    {
        var body = new
        {
            customerName = "Sneaky Customer",
            device = "Phone",
            issueDescription = "Trying to create a case owned by someone else.",
            caseType = "Repair",
            ownerId = "user-b",
            id = "case-500",
            status = "Completed"
        };

        var response = await ClientAs(FixTrackClaims.UserAToken).PostAsync(CasesUrl, Json(body));

        // Server-managed fields are unknown fields in the request, so the requester can never choose the owner.
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var details = (await ReadJsonAsync(response)).GetProperty("error").GetProperty("details");
        Assert.Contains(details.EnumerateArray(), d => d.GetProperty("field").GetString() == "ownerId");
    }

    [Fact]
    public async Task Update_OwnCase_ReplacesFields_KeepsServerFields()
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).PutAsync($"{CasesUrl}/case-101", Json(ValidUpdate("Completed")));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal("Case updated.", json.GetProperty("message").GetString());
        var data = json.GetProperty("data");
        Assert.Equal("case-101", data.GetProperty("id").GetString());
        Assert.Equal("FT-84A2", data.GetProperty("trackingCode").GetString());
        Assert.Equal("user-a", data.GetProperty("ownerId").GetString());
        Assert.Equal("Updated Customer", data.GetProperty("customerName").GetString());
        Assert.Equal("Warranty", data.GetProperty("caseType").GetString());
        Assert.Equal("Completed", data.GetProperty("status").GetString());
        Assert.Equal(JsonValueKind.String, data.GetProperty("updatedAt").ValueKind);
    }

    [Fact]
    public async Task Update_OtherUsersCase_Returns404_AndLeavesCaseUnchanged()
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).PutAsync($"{CasesUrl}/case-103", Json(ValidUpdate()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var check = await ClientAs(FixTrackClaims.UserBToken).GetAsync($"{CasesUrl}/case-103");
        Assert.Equal("Headphones", (await ReadJsonAsync(check)).GetProperty("data").GetProperty("device").GetString());
    }

    [Fact]
    public async Task Update_AdminCanUpdateAnyCase_OwnerUnchanged()
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).PutAsync($"{CasesUrl}/case-103", Json(ValidUpdate()));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("user-b", (await ReadJsonAsync(response)).GetProperty("data").GetProperty("ownerId").GetString());
    }

    [Fact]
    public async Task Update_NonexistentCase_Returns404()
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).PutAsync($"{CasesUrl}/case-999", Json(ValidUpdate()));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_OwnCase_Returns200WithNullData_ThenCaseIsGone()
    {
        var client = ClientAs(FixTrackClaims.UserBToken);

        var response = await client.DeleteAsync($"{CasesUrl}/case-104");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal("Case deleted.", json.GetProperty("message").GetString());
        Assert.Equal(JsonValueKind.Null, json.GetProperty("data").ValueKind);

        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"{CasesUrl}/case-104")).StatusCode);
    }

    [Fact]
    public async Task Delete_OtherUsersCase_Returns404_AndCaseStillExists()
    {
        var response = await ClientAs(FixTrackClaims.UserBToken).DeleteAsync($"{CasesUrl}/case-101");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await ClientAs(FixTrackClaims.UserAToken).GetAsync($"{CasesUrl}/case-101")).StatusCode);
    }

    [Fact]
    public async Task Delete_AdminCanDeleteAnyCase()
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).DeleteAsync($"{CasesUrl}/case-102");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NonexistentCase_Returns404()
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).DeleteAsync($"{CasesUrl}/case-999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_WithoutToken_Returns401()
    {
        var response = await ClientAs(null).PostAsync(CasesUrl, Json(ValidCreate));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
