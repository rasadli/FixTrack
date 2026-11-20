namespace FixTrack.Domain.Cases;

/// <summary>
/// A repair, warranty, or return case owned by a single user.
/// </summary>
/// <remarks>
/// Server-managed fields (<see cref="Id"/>, <see cref="TrackingCode"/>, <see cref="OwnerId"/>,
/// <see cref="CreatedAt"/>) are init-only and never change after creation. They are never
/// accepted from client requests. <see cref="UpdatedAt"/> is also server-managed and is set
/// by the server whenever the case is modified.
/// </remarks>
public sealed class RepairCase
{
    /// <summary>Unique case identifier, for example <c>case-101</c>. Server-managed and immutable.</summary>
    public required string Id { get; init; }

    /// <summary>Customer-facing tracking code, for example <c>FT-84A2</c>. Server-managed and immutable.</summary>
    public required string TrackingCode { get; init; }

    /// <summary>User ID of the case owner, for example <c>user-a</c>. Server-managed and immutable.</summary>
    public required string OwnerId { get; init; }

    /// <summary>When the case was created. Server-managed and immutable.</summary>
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>Name of the customer the case is for.</summary>
    public required string CustomerName { get; set; }

    /// <summary>The device being serviced, for example <c>Laptop</c>.</summary>
    public required string Device { get; set; }

    /// <summary>Description of the problem reported by the customer.</summary>
    public required string IssueDescription { get; set; }

    /// <summary>The kind of case.</summary>
    public required CaseType CaseType { get; set; }

    /// <summary>The current processing status.</summary>
    public required CaseStatus Status { get; set; }

    /// <summary>When the case was last modified, or <c>null</c> if it has never been modified. Server-managed.</summary>
    public DateTimeOffset? UpdatedAt { get; set; }
}
