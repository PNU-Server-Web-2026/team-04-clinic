using System.ComponentModel.DataAnnotations;

namespace ClinicAppointments.Api.Features.Doctors;

public record UpdateDoctorRequest
{
    [MaxLength(100)]
    public string? FirstName { get; init; }

    [MaxLength(100)]
    public string? LastName { get; init; }

    public string? Email { get; init; }

    public string? Phone { get; init; }

    [MaxLength(20)]
    public string? LicenseNumber { get; init; }

    [Range(0, 60)]
    public int? ExperienceYears { get; init; }
}
