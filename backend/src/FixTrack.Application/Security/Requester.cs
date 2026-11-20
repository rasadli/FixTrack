namespace FixTrack.Application.Security;

/// <summary>
/// The authenticated user making the current request.
/// </summary>
/// <param name="UserId">The user's ID, for example <c>user-a</c>. Used as the case owner ID.</param>
/// <param name="IsAdmin"><c>true</c> if the user has the <see cref="FixTrackClaims.AdminRole"/> role and may manage all cases.</param>
public sealed record Requester(string UserId, bool IsAdmin);
