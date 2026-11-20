namespace FixTrack.Application.Common;

/// <summary>
/// Success envelope for single-item responses: <c>{ "success": true, "message": "...", "data": { } }</c>.
/// </summary>
public sealed record ApiResponse<T>(bool Success, string Message, T? Data);
