using System.Text.Json.Serialization;

namespace FixTrack.Application.Common;

/// <summary>
/// Success envelope for single-item responses: <c>{ "success": true, "message": "...", "data": { } }</c>.
/// </summary>
/// <remarks>
/// <see cref="Data"/> is always written, even when it is <c>null</c> (for example on DELETE),
/// overriding the API-wide <c>WhenWritingNull</c> setting so the envelope shape stays consistent.
/// </remarks>
public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] T? Data);
