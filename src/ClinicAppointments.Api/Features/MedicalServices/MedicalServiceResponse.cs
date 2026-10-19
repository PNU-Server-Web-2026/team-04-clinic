namespace ClinicAppointments.Api.Features.MedicalServices;

public record MedicalServiceResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public int DurationMinutes { get; init; }

    public decimal Price { get; init; }
}
