namespace FixTrack.Application.Security;

public sealed record Requester(string UserId, bool IsAdmin);
