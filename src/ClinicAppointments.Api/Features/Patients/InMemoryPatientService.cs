using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.Patients;

public class InMemoryPatientService : IPatientService
{
    private readonly ConcurrentDictionary<int, PatientResponse> _patients = new();
    private int _nextId = 0;

    public Task<IEnumerable<PatientResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_patients.Values.AsEnumerable());
    }

    public Task<PatientResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _patients.TryGetValue(id, out var patient);
        return Task.FromResult(patient);
    }

    public Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var patient = new PatientResponse
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            Email = request.Email
        };

        _patients.TryAdd(id, patient);
        return Task.FromResult(patient);
    }

    public Task<PatientResponse?> UpdateAsync(int id, UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        if (!_patients.TryGetValue(id, out var existing))
        {
            return Task.FromResult<PatientResponse?>(null);
        }

        var updated = existing with
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            Email = request.Email
        };

        if (!_patients.TryUpdate(id, updated, existing))
        {
            return Task.FromResult<PatientResponse?>(null);
        }

        return Task.FromResult<PatientResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_patients.TryRemove(id, out _));
    }
}
