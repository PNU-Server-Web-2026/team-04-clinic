namespace ClinicAppointments.Api.Features.Cabinets;

public record CabinetResponse
{
    public int Id { get; init; }

    public string Number { get; init; } = string.Empty;

    public int Floor { get; init; }

    public string? Description { get; init; }
}
