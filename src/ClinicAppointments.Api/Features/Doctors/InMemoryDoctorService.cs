using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.Doctors;

public class InMemoryDoctorService : IDoctorService
{
    private readonly ConcurrentDictionary<int, DoctorResponse> _doctors = new();
    private int _nextId = 1;

    public Task<IEnumerable<DoctorResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(_doctors.Values.AsEnumerable());
    }

    public Task<DoctorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _doctors.TryGetValue(id, out var doctor);
        return Task.FromResult(doctor);
    }

    public Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var id = Interlocked.Increment(ref _nextId);
        var doctor = new DoctorResponse
        {
            Id = id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            LicenseNumber = request.LicenseNumber,
            ExperienceYears = request.ExperienceYears
        };

        _doctors.TryAdd(id, doctor);
        return Task.FromResult(doctor);
    }

    public Task<DoctorResponse?> UpdateAsync(int id, UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        if (!_doctors.TryGetValue(id, out var existing))
        {
            return Task.FromResult<DoctorResponse?>(null);
        }

        var updated = existing with
        {
            FirstName = request.FirstName ?? existing.FirstName,
            LastName = request.LastName ?? existing.LastName,
            Email = request.Email ?? existing.Email,
            Phone = request.Phone ?? existing.Phone,
            LicenseNumber = request.LicenseNumber ?? existing.LicenseNumber,
            ExperienceYears = request.ExperienceYears ?? existing.ExperienceYears
        };

        _doctors.TryUpdate(id, updated, existing);
        return Task.FromResult<DoctorResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_doctors.TryRemove(id, out _));
    }

    public void Clear()
    {
        _doctors.Clear();
        _nextId = 1;
    }
}
