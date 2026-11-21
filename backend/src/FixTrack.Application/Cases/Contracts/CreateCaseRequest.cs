using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases.Contracts;

public sealed record CreateCaseRequest
{
    public string? CustomerName { get; init; }

    public string? Device { get; init; }

    public string? IssueDescription { get; init; }

    public CaseType? CaseType { get; init; }
}
