namespace FixTrack.Application.Common;

public static class ErrorCodes
{
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN"; // reserved: cross-user access returns 404
    public const string NotFound = "NOT_FOUND";
    public const string MethodNotAllowed = "METHOD_NOT_ALLOWED";
    public const string UnsupportedMediaType = "UNSUPPORTED_MEDIA_TYPE";
    public const string InternalError = "INTERNAL_ERROR";
}
