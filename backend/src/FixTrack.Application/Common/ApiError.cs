namespace FixTrack.Application.Common;

public sealed record ApiError(string Code, string Message, IReadOnlyList<FieldError>? Details);
