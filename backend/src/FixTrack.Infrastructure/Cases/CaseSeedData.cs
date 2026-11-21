using FixTrack.Application.Security;
using FixTrack.Domain.Cases;

namespace FixTrack.Infrastructure.Cases;

public static class CaseSeedData
{
    public const int FirstNewIdNumber = 105;

    public static IReadOnlyList<RepairCase> Create() =>
    [
        new()
        {
            Id = "case-101",
            TrackingCode = "FT-84A2",
            OwnerId = FixTrackClaims.UserAId,
            CustomerName = "Aylin Mammadova",
            Device = "Laptop",
            IssueDescription = "Laptop does not power on after a liquid spill on the keyboard.",
            CaseType = CaseType.Repair,
            Status = CaseStatus.Open,
            CreatedAt = new DateTimeOffset(2026, 11, 2, 9, 15, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = "case-102",
            TrackingCode = "FT-3K9D",
            OwnerId = FixTrackClaims.UserAId,
            CustomerName = "Murad Huseynov",
            Device = "Smartphone",
            IssueDescription = "Touchscreen stops responding intermittently; device is under warranty.",
            CaseType = CaseType.Warranty,
            Status = CaseStatus.InProgress,
            CreatedAt = new DateTimeOffset(2026, 11, 4, 14, 30, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 11, 6, 10, 0, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = "case-103",
            TrackingCode = "FT-7Q2M",
            OwnerId = FixTrackClaims.UserBId,
            CustomerName = "Leyla Aliyeva",
            Device = "Headphones",
            IssueDescription = "Left earcup has no sound; customer requests a return.",
            CaseType = CaseType.Return,
            Status = CaseStatus.Open,
            CreatedAt = new DateTimeOffset(2026, 11, 5, 11, 45, 0, TimeSpan.Zero)
        },
        new()
        {
            Id = "case-104",
            TrackingCode = "FT-5T8X",
            OwnerId = FixTrackClaims.UserBId,
            CustomerName = "Tural Ismayilov",
            Device = "Tablet",
            IssueDescription = "Cracked screen replaced and charging port cleaned.",
            CaseType = CaseType.Repair,
            Status = CaseStatus.Completed,
            CreatedAt = new DateTimeOffset(2026, 11, 7, 16, 20, 0, TimeSpan.Zero),
            UpdatedAt = new DateTimeOffset(2026, 11, 9, 12, 0, 0, TimeSpan.Zero)
        }
    ];
}
