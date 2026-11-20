namespace FixTrack.Application.Common;

/// <summary>
/// Error information inside an <see cref="ApiErrorResponse"/>.
/// </summary>
/// <param name="Code">One of the <see cref="ErrorCodes"/> constants.</param>
/// <param name="Message">Human-readable message. Never contains internal exception details.</param>
/// <param name="Details">Field-level problems. Non-null only for <see cref="ErrorCodes.ValidationError"/>.</param>
public sealed record ApiError(string Code, string Message, IReadOnlyList<FieldError>? Details);
