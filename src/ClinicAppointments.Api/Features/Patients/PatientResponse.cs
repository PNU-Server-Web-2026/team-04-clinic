namespace ClinicAppointments.Api.Features.Patients;

public record PatientResponse
{
    public int Id { get; init; }

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public DateOnly BirthDate { get; init; }

    public string? Phone { get; init; }

    public string? Email { get; init; }
}
