namespace FixTrack.Application.Security;

/// <summary>
/// Claim types, roles, scheme name and mock bearer tokens shared by the authentication
/// handler, the requester abstraction, and the tests.
/// </summary>
/// <remarks>
/// Assignment 2 uses predefined mock tokens instead of real authentication.
/// </remarks>
public static class FixTrackClaims
{
    /// <summary>Claim type that holds the user ID.</summary>
    public const string UserId = "sub";

    /// <summary>Role name that grants access to all cases.</summary>
    public const string AdminRole = "Admin";

    /// <summary>Name of the mock bearer authentication scheme.</summary>
    public const string SchemeName = "MockBearer";

    /// <summary>Bearer token for regular User A.</summary>
    public const string UserAToken = "user-a-token";

    /// <summary>Bearer token for regular User B.</summary>
    public const string UserBToken = "user-b-token";

    /// <summary>Bearer token for the administrator.</summary>
    public const string AdminToken = "admin-token";

    /// <summary>User ID for <see cref="UserAToken"/>.</summary>
    public const string UserAId = "user-a";

    /// <summary>User ID for <see cref="UserBToken"/>.</summary>
    public const string UserBId = "user-b";

    /// <summary>User ID for <see cref="AdminToken"/>.</summary>
    public const string AdminId = "admin";

    /// <summary>Maps each mock token to the user ID it authenticates as.</summary>
    public static readonly IReadOnlyDictionary<string, string> TokenToUserId = new Dictionary<string, string>
    {
        [UserAToken] = UserAId,
        [UserBToken] = UserBId,
        [AdminToken] = AdminId
    };

    /// <summary>Returns <c>true</c> if <paramref name="userId"/> has the <see cref="AdminRole"/> role.</summary>
    public static bool IsAdminUser(string userId) => userId == AdminId;
}
