namespace FixTrack.Application.Common;

/// <summary>
/// Machine-readable error codes used in <see cref="ApiError.Code"/>.
/// </summary>
public static class ErrorCodes
{
    /// <summary>400: the request body or query parameters are invalid.</summary>
    public const string ValidationError = "VALIDATION_ERROR";

    /// <summary>401: the bearer token is missing or invalid.</summary>
    public const string Unauthorized = "UNAUTHORIZED";

    /// <summary>403: reserved. Cross-user access returns 404 instead, so this is not currently used for cases.</summary>
    public const string Forbidden = "FORBIDDEN";

    /// <summary>404: the resource does not exist or is not visible to the requester.</summary>
    public const string NotFound = "NOT_FOUND";

    /// <summary>405: the HTTP method is not supported for this route.</summary>
    public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";

    /// <summary>415: the request body is not JSON.</summary>
    public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE";

    /// <summary>500: an unexpected server error occurred.</summary>
    public const string InternalError = "INTERNAL_ERROR";
}
