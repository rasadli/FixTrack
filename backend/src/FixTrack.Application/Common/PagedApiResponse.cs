namespace FixTrack.Application.Common;

public sealed record PagedApiResponse<T>(bool Success, string Message, IReadOnlyList<T> Data, PageMeta Meta);
