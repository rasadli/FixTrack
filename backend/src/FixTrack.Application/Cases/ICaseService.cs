using FixTrack.Application.Cases.Contracts;

namespace FixTrack.Application.Cases;

public interface ICaseService
{
    CasePage List(int page, int perPage);

    CaseResponse? Get(string id);

    CaseResponse Create(CreateCaseRequest request);

    CaseResponse? Update(string id, UpdateCaseRequest request);

    bool Delete(string id);
}

public sealed record CasePage(IReadOnlyList<CaseResponse> Items, int Total);
