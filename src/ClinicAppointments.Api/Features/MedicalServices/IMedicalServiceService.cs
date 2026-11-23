namespace ClinicAppointments.Api.Features.MedicalServices;

public interface IMedicalServiceService
{
    Task<IEnumerable<MedicalServiceResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<MedicalServiceResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<MedicalServiceResponse> CreateAsync(CreateMedicalServiceRequest request, CancellationToken cancellationToken);

    Task<MedicalServiceResponse?> UpdateAsync(int id, UpdateMedicalServiceRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
