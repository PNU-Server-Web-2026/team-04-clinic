using System.Collections.Concurrent;
using System.Threading;

namespace ClinicAppointments.Api.Features.Doctors;

public class InMemoryDoctorService : IDoctorService
{
    private readonly ConcurrentDictionary<int, DoctorResponse> _doctors = new();
    private int _nextId = 0;

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
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            LicenseNumber = request.LicenseNumber,
            ExperienceYears = request.ExperienceYears
        };

        if (!_doctors.TryUpdate(id, updated, existing))
        {
            return Task.FromResult<DoctorResponse?>(null);
        }

        return Task.FromResult<DoctorResponse?>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        return Task.FromResult(_doctors.TryRemove(id, out _));
    }
}
