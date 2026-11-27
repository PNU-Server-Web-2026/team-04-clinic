using System.ComponentModel.DataAnnotations;
using ClinicAppointments.Api.Common.Validation;

namespace ClinicAppointments.Api.Features.MedicalServices;

public record CreateMedicalServiceRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Range(10, 240)]
    [MultipleOf(5)]
    public int DurationMinutes { get; init; }

    [Range(0, double.MaxValue)]
    public decimal Price { get; init; }
}
