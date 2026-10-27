namespace ClinicAppointments.Api.Features.Doctors;

public interface IDoctorService
{
    Task<IEnumerable<DoctorResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<DoctorResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<DoctorResponse> CreateAsync(CreateDoctorRequest request, CancellationToken cancellationToken);

    Task<DoctorResponse?> UpdateAsync(int id, UpdateDoctorRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
