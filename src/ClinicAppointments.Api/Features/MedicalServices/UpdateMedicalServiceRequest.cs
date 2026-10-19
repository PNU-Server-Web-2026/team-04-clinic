using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.MedicalServices;

public record UpdateMedicalServiceRequest
{
    [MaxLength(200)]
    public string? Name { get; init; }

    [Range(10, 240)]
    public int? DurationMinutes { get; init; }

    [Range(0, double.MaxValue)]
    public decimal? Price { get; init; }
}
