namespace FixTrack.Application.Common;

/// <summary>
/// Success envelope for list responses, with pagination metadata in <see cref="Meta"/>.
/// </summary>
public sealed record PagedApiResponse<T>(bool Success, string Message, IReadOnlyList<T> Data, PageMeta Meta);
