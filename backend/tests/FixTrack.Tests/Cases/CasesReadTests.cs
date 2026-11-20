using System.Net;
using FixTrack.Application.Security;

namespace FixTrack.Tests.Cases;

public sealed class CasesReadTests : CasesTestBase
{
    [Theory]
    [InlineData(FixTrackClaims.UserAToken, new[] { "case-101", "case-102" })]
    [InlineData(FixTrackClaims.UserBToken, new[] { "case-103", "case-104" })]
    [InlineData(FixTrackClaims.AdminToken, new[] { "case-101", "case-102", "case-103", "case-104" })]
    public async Task List_ReturnsOnlyVisibleCases_InOrder(string token, string[] expectedIds)
    {
        var response = await ClientAs(token).GetAsync(CasesUrl);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal("Cases retrieved.", json.GetProperty("message").GetString());
        Assert.Equal(expectedIds, json.GetProperty("data").EnumerateArray().Select(c => c.GetProperty("id").GetString()));

        var meta = json.GetProperty("meta");
        Assert.Equal(expectedIds.Length, meta.GetProperty("total").GetInt32());
        Assert.Equal(1, meta.GetProperty("page").GetInt32());
        Assert.Equal(10, meta.GetProperty("perPage").GetInt32());
        Assert.Equal(1, meta.GetProperty("totalPages").GetInt32());
    }

    [Fact]
    public async Task List_Paginates()
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).GetAsync($"{CasesUrl}?page=2&perPage=3");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Equal(["case-104"], json.GetProperty("data").EnumerateArray().Select(c => c.GetProperty("id").GetString()));
        var meta = json.GetProperty("meta");
        Assert.Equal(4, meta.GetProperty("total").GetInt32());
        Assert.Equal(2, meta.GetProperty("page").GetInt32());
        Assert.Equal(3, meta.GetProperty("perPage").GetInt32());
        Assert.Equal(2, meta.GetProperty("totalPages").GetInt32());
    }

    [Fact]
    public async Task List_PageBeyondEnd_ReturnsEmptyData()
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).GetAsync($"{CasesUrl}?page=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.Empty(json.GetProperty("data").EnumerateArray());
        Assert.Equal(2, json.GetProperty("meta").GetProperty("total").GetInt32());
    }

    [Theory]
    [InlineData("page=0", "page")]
    [InlineData("page=-1", "page")]
    [InlineData("page=abc", "page")]
    [InlineData("perPage=0", "perPage")]
    [InlineData("perPage=51", "perPage")]
    [InlineData("perPage=1.5", "perPage")]
    public async Task List_InvalidPagination_Returns400(string query, string field)
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).GetAsync($"{CasesUrl}?{query}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.False(json.GetProperty("success").GetBoolean());
        var error = json.GetProperty("error");
        Assert.Equal("VALIDATION_ERROR", error.GetProperty("code").GetString());
        Assert.Equal(field, error.GetProperty("details")[0].GetProperty("field").GetString());
    }

    [Fact]
    public async Task Get_OwnCase_Returns200WithCase()
    {
        var response = await ClientAs(FixTrackClaims.UserAToken).GetAsync($"{CasesUrl}/case-101");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.True(json.GetProperty("success").GetBoolean());
        Assert.Equal("Case retrieved.", json.GetProperty("message").GetString());
        var data = json.GetProperty("data");
        Assert.Equal("case-101", data.GetProperty("id").GetString());
        Assert.Equal("FT-84A2", data.GetProperty("trackingCode").GetString());
        Assert.Equal("user-a", data.GetProperty("ownerId").GetString());
        Assert.Equal("Laptop", data.GetProperty("device").GetString());
        Assert.Equal("Repair", data.GetProperty("caseType").GetString());
        Assert.Equal("Open", data.GetProperty("status").GetString());
    }

    [Theory]
    [InlineData(FixTrackClaims.UserAToken, "case-103")]
    [InlineData(FixTrackClaims.UserBToken, "case-101")]
    public async Task Get_OtherUsersCase_Returns404(string token, string id)
    {
        var response = await ClientAs(token).GetAsync($"{CasesUrl}/{id}");

        await AssertCaseNotFound(response);
    }

    [Theory]
    [InlineData("case-101")]
    [InlineData("case-103")]
    public async Task Get_AdminCanReadAnyCase(string id)
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).GetAsync($"{CasesUrl}/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_NonexistentCase_Returns404()
    {
        var response = await ClientAs(FixTrackClaims.AdminToken).GetAsync($"{CasesUrl}/case-999");

        await AssertCaseNotFound(response);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("not-a-real-token")]
    public async Task List_WithoutValidToken_Returns401(string? token)
    {
        var response = await ClientAs(token).GetAsync(CasesUrl);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private static async Task AssertCaseNotFound(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var json = await ReadJsonAsync(response);
        Assert.False(json.GetProperty("success").GetBoolean());
        var error = json.GetProperty("error");
        Assert.Equal("NOT_FOUND", error.GetProperty("code").GetString());
        Assert.Equal("Case not found.", error.GetProperty("message").GetString());
        Assert.False(error.TryGetProperty("details", out _));
    }
}
