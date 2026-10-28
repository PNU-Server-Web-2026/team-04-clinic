namespace ClinicAppointments.Api.Features.Specializations;

public interface ISpecializationService
{
    Task<IReadOnlyCollection<SpecializationResponse>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<SpecializationResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<SpecializationResponse> CreateAsync(
        CreateSpecializationRequest request,
        CancellationToken cancellationToken);

    Task<bool> UpdateAsync(
        int id,
        UpdateSpecializationRequest request,
        CancellationToken cancellationToken);

    Task<bool> DeleteAsync(
        int id,
        CancellationToken cancellationToken);
}