using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.Patients;

public record CreatePatientRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }
}
