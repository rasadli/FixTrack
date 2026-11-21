namespace FixTrack.Application.Common;

public static class ApiErrors
{
    public static ApiErrorResponse Validation(IReadOnlyList<FieldError> details) =>
        Create(ErrorCodes.ValidationError, "Invalid request.", details);

    public static ApiErrorResponse Unauthorized(string message) =>
        Create(ErrorCodes.Unauthorized, message);

    public static ApiErrorResponse NotFound(string message) =>
        Create(ErrorCodes.NotFound, message);

    public static ApiErrorResponse MethodNotAllowed() =>
        Create(ErrorCodes.MethodNotAllowed, "The HTTP method is not allowed for this resource.");

    public static ApiErrorResponse UnsupportedMediaType() =>
        Create(ErrorCodes.UnsupportedMediaType, "The request body must be JSON (Content-Type: application/json).");

    public static ApiErrorResponse Internal() =>
        Create(ErrorCodes.InternalError, "An unexpected error occurred.");

    private static ApiErrorResponse Create(string code, string message, IReadOnlyList<FieldError>? details = null) =>
        new(false, new ApiError(code, message, details));
}
