namespace ClinicAppointments.Api.Features.Patients;

public interface IPatientService
{
    Task<IEnumerable<PatientResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<PatientResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<PatientResponse> CreateAsync(CreatePatientRequest request, CancellationToken cancellationToken);

    Task<PatientResponse?> UpdateAsync(int id, UpdatePatientRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
