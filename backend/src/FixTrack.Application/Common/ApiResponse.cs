using System.Text.Json.Serialization;

namespace FixTrack.Application.Common;

// Data is always written, even when null (DELETE), despite the global WhenWritingNull setting.
public sealed record ApiResponse<T>(
    bool Success,
    string Message,
    [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] T? Data);
