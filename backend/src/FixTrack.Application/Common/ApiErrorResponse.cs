namespace FixTrack.Application.Common;

/// <summary>
/// Error envelope: <c>{ "success": false, "error": { "code": "...", "message": "...", "details": [ ] } }</c>.
/// </summary>
public sealed record ApiErrorResponse(bool Success, ApiError Error);
