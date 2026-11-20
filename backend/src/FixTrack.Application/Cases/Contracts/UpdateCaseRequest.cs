using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases.Contracts;

/// <summary>
/// Request body for <c>PUT /api/v1/cases/{id}</c>.
/// </summary>
/// <remarks>
/// PUT is a full replacement of these five fields: every field must be supplied.
/// Server-managed fields (id, tracking code, owner, timestamps) are intentionally absent,
/// so ownership cannot be changed through the request body.
/// Properties are nullable so that missing values can be reported as validation errors.
/// </remarks>
public sealed record UpdateCaseRequest
{
    public string? CustomerName { get; init; }

    public string? Device { get; init; }

    public string? IssueDescription { get; init; }

    public CaseType? CaseType { get; init; }

    public CaseStatus? Status { get; init; }
}
