namespace FixTrack.Application.Common;

/// <summary>
/// Factory helpers for success envelopes.
/// </summary>
public static class ApiResponses
{
    /// <summary>Creates a single-item success envelope.</summary>
    public static ApiResponse<T> Ok<T>(string message, T? data) => new(true, message, data);

    /// <summary>
    /// Creates a list success envelope. <see cref="PageMeta.TotalPages"/> is
    /// ceiling(<paramref name="total"/> / <paramref name="perPage"/>), or 0 when <paramref name="total"/> is 0.
    /// </summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="perPage"/> is less than 1.</exception>
    public static PagedApiResponse<T> Paged<T>(string message, IReadOnlyList<T> items, int total, int page, int perPage)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);

        var totalPages = total == 0 ? 0 : (total + perPage - 1) / perPage;
        return new PagedApiResponse<T>(true, message, items, new PageMeta(total, page, perPage, totalPages));
    }
}
