using FixTrack.Application.Cases.Contracts;
using FixTrack.Application.Security;
using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases;

/// <inheritdoc cref="ICaseService"/>
public sealed class CaseService(
    ICaseRepository repository,
    ICurrentRequester currentRequester,
    TimeProvider timeProvider) : ICaseService
{
    public CasePage List(int page, int perPage)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(perPage, 1);

        var requester = currentRequester.Get();
        var visible = repository.GetAll()
            .Where(c => CanAccess(requester, c))
            .OrderBy(c => c.CreatedAt)
            .ThenBy(c => c.Id, StringComparer.Ordinal)
            .ToList();

        var items = visible
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .Select(CaseResponse.From)
            .ToList();

        return new CasePage(items, visible.Count);
    }

    public CaseResponse? Get(string id)
    {
        var repairCase = FindVisible(id);
        return repairCase is null ? null : CaseResponse.From(repairCase);
    }

    public CaseResponse Create(CreateCaseRequest request)
    {
        var requester = currentRequester.Get();

        var repairCase = new RepairCase
        {
            Id = repository.NextId(),
            TrackingCode = repository.NewTrackingCode(),
            // Ownership always comes from the authenticated requester, never from the request body.
            OwnerId = requester.UserId,
            CreatedAt = timeProvider.GetUtcNow(),
            CustomerName = Required(request.CustomerName, nameof(request.CustomerName)),
            Device = Required(request.Device, nameof(request.Device)),
            IssueDescription = Required(request.IssueDescription, nameof(request.IssueDescription)),
            CaseType = Required(request.CaseType, nameof(request.CaseType)),
            Status = CaseStatus.Open
        };

        repository.Add(repairCase);
        return CaseResponse.From(repairCase);
    }

    public CaseResponse? Update(string id, UpdateCaseRequest request)
    {
        var repairCase = FindVisible(id);
        if (repairCase is null)
        {
            return null;
        }

        // Full replacement of the five editable fields. Id, TrackingCode, OwnerId and CreatedAt never change.
        repairCase.CustomerName = Required(request.CustomerName, nameof(request.CustomerName));
        repairCase.Device = Required(request.Device, nameof(request.Device));
        repairCase.IssueDescription = Required(request.IssueDescription, nameof(request.IssueDescription));
        repairCase.CaseType = Required(request.CaseType, nameof(request.CaseType));
        repairCase.Status = Required(request.Status, nameof(request.Status));
        repairCase.UpdatedAt = timeProvider.GetUtcNow();

        return CaseResponse.From(repairCase);
    }

    public bool Delete(string id) => FindVisible(id) is not null && repository.Remove(id);

    private RepairCase? FindVisible(string id)
    {
        var requester = currentRequester.Get();
        var repairCase = repository.GetById(id);
        return repairCase is not null && CanAccess(requester, repairCase) ? repairCase : null;
    }

    private static bool CanAccess(Requester requester, RepairCase repairCase) =>
        requester.IsAdmin || repairCase.OwnerId == requester.UserId;

    // Validation happens before the service is called (feature/mock-auth-validation).
    // These guards only protect the domain model if an unvalidated request slips through.
    private static string Required(string? value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new ArgumentException($"{name} is required.", name) : value.Trim();

    private static T Required<T>(T? value, string name) where T : struct =>
        value ?? throw new ArgumentException($"{name} is required.", name);
}
