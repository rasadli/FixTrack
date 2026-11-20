using System.Collections.Concurrent;
using FixTrack.Application.Cases;
using FixTrack.Domain.Cases;

namespace FixTrack.Infrastructure.Cases;

/// <summary>
/// Thread-safe, process-local case store. Register as a singleton.
/// Starts with <see cref="CaseSeedData"/>; all data resets when the application restarts.
/// </summary>
public sealed class InMemoryCaseRepository : ICaseRepository
{
    private const string TrackingCodeAlphabet = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

    private readonly ConcurrentDictionary<string, RepairCase> _cases = new(StringComparer.Ordinal);
    private int _lastIdNumber = CaseSeedData.FirstNewIdNumber - 1;

    public InMemoryCaseRepository()
    {
        foreach (var seed in CaseSeedData.Create())
        {
            _cases[seed.Id] = seed;
        }
    }

    public IReadOnlyList<RepairCase> GetAll() => _cases.Values.ToList();

    public RepairCase? GetById(string id) => _cases.GetValueOrDefault(id);

    public string NextId() => $"case-{Interlocked.Increment(ref _lastIdNumber)}";

    public string NewTrackingCode()
    {
        while (true)
        {
            var code = "FT-" + new string(Random.Shared.GetItems(TrackingCodeAlphabet.AsSpan(), 4));
            if (_cases.Values.All(c => c.TrackingCode != code))
            {
                return code;
            }
        }
    }

    public void Add(RepairCase repairCase)
    {
        if (!_cases.TryAdd(repairCase.Id, repairCase))
        {
            throw new InvalidOperationException($"A case with ID '{repairCase.Id}' already exists.");
        }
    }

    public bool Remove(string id) => _cases.TryRemove(id, out _);
}
