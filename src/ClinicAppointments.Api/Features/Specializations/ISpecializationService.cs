namespace ClinicAppointments.Api.Features.Specializations;

public interface ISpecializationService
{
    Task<IEnumerable<SpecializationResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<SpecializationResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<SpecializationResponse> CreateAsync(CreateSpecializationRequest request, CancellationToken cancellationToken);

    Task<SpecializationResponse?> UpdateAsync(int id, UpdateSpecializationRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
