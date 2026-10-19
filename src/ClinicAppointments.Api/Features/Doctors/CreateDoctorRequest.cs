using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.Doctors;

public record CreateDoctorRequest
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? Phone { get; init; }

    [MaxLength(20)]
    public string LicenseNumber { get; init; } = string.Empty;

    [Range(0, 60)]
    public int ExperienceYears { get; init; }
}
