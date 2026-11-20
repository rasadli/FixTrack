namespace FixTrack.Application.Common;

/// <summary>
/// Pagination metadata for list responses.
/// </summary>
/// <param name="Total">Total number of items across all pages.</param>
/// <param name="Page">Current page number, starting at 1.</param>
/// <param name="PerPage">Maximum number of items per page.</param>
/// <param name="TotalPages">Number of pages; 0 when there are no items.</param>
public sealed record PageMeta(int Total, int Page, int PerPage, int TotalPages);
