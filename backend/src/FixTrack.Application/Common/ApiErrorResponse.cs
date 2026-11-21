namespace FixTrack.Application.Common;

public sealed record ApiErrorResponse(bool Success, ApiError Error);
