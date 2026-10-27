namespace ClinicAppointments.Api.Features.Doctors;

public record DoctorResponse
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? Phone { get; init; }

    public string LicenseNumber { get; init; } = string.Empty;

    public int ExperienceYears { get; init; }
}
