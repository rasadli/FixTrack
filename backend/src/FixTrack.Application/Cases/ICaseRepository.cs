using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases;

/// <summary>
/// Storage for repair cases. Does not enforce ownership; that is the job of <see cref="ICaseService"/>.
/// </summary>
public interface ICaseRepository
{
    /// <summary>Returns a snapshot of all stored cases, in no particular order.</summary>
    IReadOnlyList<RepairCase> GetAll();

    /// <summary>Returns the case with the given ID, or <c>null</c> if it does not exist.</summary>
    RepairCase? GetById(string id);

    /// <summary>Returns the next unused case ID, for example <c>case-105</c>.</summary>
    string NextId();

    /// <summary>Returns a new tracking code, for example <c>FT-84A2</c>, not used by any stored case.</summary>
    string NewTrackingCode();

    /// <summary>Stores a new case.</summary>
    /// <exception cref="InvalidOperationException">A case with the same ID already exists.</exception>
    void Add(RepairCase repairCase);

    /// <summary>Removes the case with the given ID. Returns <c>false</c> if it did not exist.</summary>
    bool Remove(string id);
}
