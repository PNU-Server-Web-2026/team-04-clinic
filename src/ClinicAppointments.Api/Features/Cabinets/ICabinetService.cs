namespace ClinicAppointments.Api.Features.Cabinets;

public interface ICabinetService
{
    Task<IEnumerable<CabinetResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<CabinetResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);

    Task<CabinetResponse> CreateAsync(CreateCabinetRequest request, CancellationToken cancellationToken);

    Task<CabinetResponse?> UpdateAsync(int id, UpdateCabinetRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
