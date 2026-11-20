using FixTrack.Application.Cases.Contracts;

namespace FixTrack.Application.Cases;

/// <summary>
/// Case operations for the current requester (see <see cref="Security.ICurrentRequester"/>).
/// </summary>
/// <remarks>
/// Ownership rule: regular users only see and change their own cases; the admin sees and changes all cases.
/// A case owned by someone else is treated exactly like a case that does not exist (<c>null</c> / <c>false</c>),
/// so the API returns 404 and does not reveal that the case exists.
/// Request DTOs are expected to be validated before they reach the service.
/// </remarks>
public interface ICaseService
{
    /// <summary>Returns one page of visible cases, ordered by <c>CreatedAt</c> then <c>Id</c>, plus the total count.</summary>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="page"/> or <paramref name="perPage"/> is less than 1.</exception>
    CasePage List(int page, int perPage);

    /// <summary>Returns the case, or <c>null</c> if it does not exist or is not visible to the requester.</summary>
    CaseResponse? Get(string id);

    /// <summary>Creates a new <c>Open</c> case owned by the requester.</summary>
    CaseResponse Create(CreateCaseRequest request);

    /// <summary>Replaces the editable fields, or returns <c>null</c> if the case does not exist or is not visible.</summary>
    CaseResponse? Update(string id, UpdateCaseRequest request);

    /// <summary>Deletes the case. Returns <c>false</c> if it does not exist or is not visible.</summary>
    bool Delete(string id);
}

/// <summary>One page of cases and the total number of visible cases.</summary>
public sealed record CasePage(IReadOnlyList<CaseResponse> Items, int Total);
