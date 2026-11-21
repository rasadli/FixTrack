namespace FixTrack.Application.Security;

public static class FixTrackClaims
{
    public const string UserId = "sub";
    public const string AdminRole = "Admin";
    public const string SchemeName = "MockBearer";

    public const string UserAToken = "user-a-token";
    public const string UserBToken = "user-b-token";
    public const string AdminToken = "admin-token";

    public const string UserAId = "user-a";
    public const string UserBId = "user-b";
    public const string AdminId = "admin";

    public static readonly IReadOnlyDictionary<string, string> TokenToUserId = new Dictionary<string, string>
    {
        [UserAToken] = UserAId,
        [UserBToken] = UserBId,
        [AdminToken] = AdminId
    };

    public static bool IsAdminUser(string userId) => userId == AdminId;
}
