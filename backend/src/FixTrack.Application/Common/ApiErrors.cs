namespace FixTrack.Application.Common;

/// <summary>
/// Factory helpers for error envelopes. Every result has <c>Success = false</c>, and
/// <see cref="ApiError.Details"/> is non-null only for validation errors.
/// </summary>
public static class ApiErrors
{
    /// <summary>400 <see cref="ErrorCodes.ValidationError"/> with field-level details.</summary>
    public static ApiErrorResponse Validation(IReadOnlyList<FieldError> details) =>
        Create(ErrorCodes.ValidationError, "Invalid request.", details);

    /// <summary>401 <see cref="ErrorCodes.Unauthorized"/>.</summary>
    public static ApiErrorResponse Unauthorized(string message) =>
        Create(ErrorCodes.Unauthorized, message);

    /// <summary>404 <see cref="ErrorCodes.NotFound"/>. Use <see cref="Messages"/> for the message.</summary>
    public static ApiErrorResponse NotFound(string message) =>
        Create(ErrorCodes.NotFound, message);

    /// <summary>405 <see cref="ErrorCodes.MethodNotAllowed"/>.</summary>
    public static ApiErrorResponse MethodNotAllowed() =>
        Create(ErrorCodes.MethodNotAllowed, "The HTTP method is not allowed for this resource.");

    /// <summary>415 <see cref="ErrorCodes.UnsupportedMediaType"/>.</summary>
    public static ApiErrorResponse UnsupportedMediaType() =>
        Create(ErrorCodes.UnsupportedMediaType, "The request body must be JSON (Content-Type: application/json).");

    /// <summary>500 <see cref="ErrorCodes.InternalError"/>. Never includes exception details.</summary>
    public static ApiErrorResponse Internal() =>
        Create(ErrorCodes.InternalError, "An unexpected error occurred.");

    private static ApiErrorResponse Create(string code, string message, IReadOnlyList<FieldError>? details = null) =>
        new(false, new ApiError(code, message, details));
}
