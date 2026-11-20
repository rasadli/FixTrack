namespace FixTrack.Application.Common;

/// <summary>
/// Standard messages shared across the API so that responses stay consistent.
/// </summary>
public static class Messages
{
    /// <summary>Returned when a case does not exist or belongs to another user.</summary>
    public const string CaseNotFound = "Case not found.";

    /// <summary>Returned when no route matches the request.</summary>
    public const string ResourceNotFound = "The requested resource does not exist.";
}
