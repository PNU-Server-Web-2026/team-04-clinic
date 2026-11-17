using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.MedicalServices;

public class InMemoryMedicalServiceService : IMedicalServiceService
{
    private readonly ConcurrentDictionary<int, MedicalServiceResponse> _services = new();
    private int _nextId = 0;

    public Task<IEnumerable<MedicalServiceResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_services.Values.AsEnumerable());
    }

    public Task<MedicalServiceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _services.TryGetValue(id, out var service);
        return Task.FromResult(service);
    }

    public Task<MedicalServiceResponse> CreateAsync(CreateMedicalServiceRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var service = new MedicalServiceResponse
        {
            Id = id,
            Name = request.Name,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price
        };

        _services.TryAdd(id, service);
        return Task.FromResult(service);
    }

    public Task<MedicalServiceResponse?> UpdateAsync(int id, UpdateMedicalServiceRequest request, CancellationToken cancellationToken)
    {
        if (!_services.TryGetValue(id, out var existing))
        {
            return Task.FromResult<MedicalServiceResponse?>(null);
        }

        var updated = existing with
        {
            Name = request.Name,
            DurationMinutes = request.DurationMinutes,
            Price = request.Price
        };

        if (!_services.TryUpdate(id, updated, existing))
        {
            return Task.FromResult<MedicalServiceResponse?>(null);
        }

        return Task.FromResult<MedicalServiceResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_services.TryRemove(id, out _));
    }
}
