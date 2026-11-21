namespace FixTrack.Application.Common;

public static class ApiResponses
{
    public static ApiResponse<T> Ok<T>(string message, T? data) => new(true, message, data);

    public static PagedApiResponse<T> Paged<T>(string message, IReadOnlyList<T> items, int total, int page, int perPage)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);

        var totalPages = total == 0 ? 0 : (total + perPage - 1) / perPage;
        return new PagedApiResponse<T>(true, message, items, new PageMeta(total, page, perPage, totalPages));
    }
}
