using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases.Contracts;

/// <summary>
/// Request body for <c>POST /api/v1/cases</c>.
/// </summary>
/// <remarks>
/// New cases always start as <see cref="CaseStatus.Open"/>, so there is no status field.
/// Server-managed fields (id, tracking code, owner, timestamps) are intentionally absent.
/// Properties are nullable so that missing values can be reported as validation errors.
/// </remarks>
public sealed record CreateCaseRequest
{
    public string? CustomerName { get; init; }

    public string? Device { get; init; }

    public string? IssueDescription { get; init; }

    public CaseType? CaseType { get; init; }
}
