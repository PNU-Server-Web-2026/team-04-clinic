namespace ClinicAppointments.Api.Features.Doctors;

/// <summary>
/// Лікар клініки. Таблицю і обмеження задає <see cref="DoctorConfiguration"/>.
/// </summary>
public class Doctor
{
    public int Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string LicenseNumber { get; set; } = string.Empty;

    public int ExperienceYears { get; set; }
}
