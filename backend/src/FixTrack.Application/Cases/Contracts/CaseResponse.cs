using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases.Contracts;

public sealed record CaseResponse
{
    public required string Id { get; init; }

    public required string TrackingCode { get; init; }

    public required string OwnerId { get; init; }

    public required string CustomerName { get; init; }

    public required string Device { get; init; }

    public required string IssueDescription { get; init; }

    public required CaseType CaseType { get; init; }

    public required CaseStatus Status { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? UpdatedAt { get; init; }

    public static CaseResponse From(RepairCase c) => new()
    {
        Id = c.Id,
        TrackingCode = c.TrackingCode,
        OwnerId = c.OwnerId,
        CustomerName = c.CustomerName,
        Device = c.Device,
        IssueDescription = c.IssueDescription,
        CaseType = c.CaseType,
        Status = c.Status,
        CreatedAt = c.CreatedAt,
        UpdatedAt = c.UpdatedAt
    };
}
