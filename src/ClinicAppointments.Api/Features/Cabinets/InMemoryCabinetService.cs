using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.Cabinets;

public class InMemoryCabinetService : ICabinetService
{
    private readonly ConcurrentDictionary<int, CabinetResponse> _cabinets = new();
    private int _nextId = 0;

    public Task<IEnumerable<CabinetResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_cabinets.Values.AsEnumerable());
    }

    public Task<CabinetResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _cabinets.TryGetValue(id, out var cabinet);
        return Task.FromResult(cabinet);
    }

    public Task<CabinetResponse> CreateAsync(CreateCabinetRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var cabinet = new CabinetResponse
        {
            Id = id,
            Number = request.Number,
            Floor = request.Floor,
            Description = request.Description
        };

        _cabinets.TryAdd(id, cabinet);
        return Task.FromResult(cabinet);
    }

    public Task<CabinetResponse?> UpdateAsync(int id, UpdateCabinetRequest request, CancellationToken cancellationToken)
    {
        if (!_cabinets.TryGetValue(id, out var existing))
        {
            return Task.FromResult<CabinetResponse?>(null);
        }

        var updated = existing with
        {
            Number = request.Number,
            Floor = request.Floor,
            Description = request.Description
        };

        if (!_cabinets.TryUpdate(id, updated, existing))
        {
            return Task.FromResult<CabinetResponse?>(null);
        }

        return Task.FromResult<CabinetResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_cabinets.TryRemove(id, out _));
    }
}
