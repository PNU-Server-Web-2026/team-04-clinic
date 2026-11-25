namespace ClinicAppointments.Api.Features.MedicalServices;

public sealed class MedicalService
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int DurationMinutes { get; set; }

    public decimal Price { get; set; }
}
