using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.Specializations;

public class InMemorySpecializationService : ISpecializationService
{
    private readonly ConcurrentDictionary<int, SpecializationResponse> _specializations = new();
    private int _nextId = 0;

    public Task<IEnumerable<SpecializationResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_specializations.Values.AsEnumerable());
    }

    public Task<SpecializationResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _specializations.TryGetValue(id, out var specialization);
        return Task.FromResult(specialization);
    }

    public Task<SpecializationResponse> CreateAsync(CreateSpecializationRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var specialization = new SpecializationResponse
        {
            Id = id,
            Name = request.Name,
            Description = request.Description
        };

        _specializations.TryAdd(id, specialization);
        return Task.FromResult(specialization);
    }

    public Task<SpecializationResponse?> UpdateAsync(int id, UpdateSpecializationRequest request, CancellationToken cancellationToken)
    {
        while (true)
        {
            if (!_specializations.TryGetValue(id, out var existing))
            {
                return Task.FromResult<SpecializationResponse?>(null);
            }

            var updated = existing with
            {
                Name = request.Name,
                Description = request.Description
            };

            if (_specializations.TryUpdate(id, updated, existing))
            {
                return Task.FromResult<SpecializationResponse?>(updated);
            }
        }
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_specializations.TryRemove(id, out _));
    }
}
