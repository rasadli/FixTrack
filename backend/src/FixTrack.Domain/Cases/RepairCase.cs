namespace FixTrack.Domain.Cases;

public sealed class RepairCase
{
    public required string Id { get; init; }

    public required string TrackingCode { get; init; }

    public required string OwnerId { get; init; }

    public required DateTimeOffset CreatedAt { get; init; }

    public required string CustomerName { get; set; }

    public required string Device { get; set; }

    public required string IssueDescription { get; set; }

    public required CaseType CaseType { get; set; }

    public required CaseStatus Status { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }
}
