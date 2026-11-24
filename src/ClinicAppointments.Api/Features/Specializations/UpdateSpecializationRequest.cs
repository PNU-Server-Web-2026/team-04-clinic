using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.Specializations;

public record UpdateSpecializationRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(1000)]
    public string? Description { get; init; }
}
