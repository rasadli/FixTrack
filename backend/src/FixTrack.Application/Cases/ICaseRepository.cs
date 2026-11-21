using FixTrack.Domain.Cases;

namespace FixTrack.Application.Cases;

public interface ICaseRepository
{
    IReadOnlyList<RepairCase> GetAll();

    RepairCase? GetById(string id);

    string NextId();

    string NewTrackingCode();

    void Add(RepairCase repairCase);

    bool Remove(string id);
}
