using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.Cabinets;

public record UpdateCabinetRequest
{
    [Required]
    [MaxLength(10)]
    public string Number { get; init; } = string.Empty;

    public int Floor { get; init; }

    [MaxLength(500)]
    public string? Description { get; init; }
}
