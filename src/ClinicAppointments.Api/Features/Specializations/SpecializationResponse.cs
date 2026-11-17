namespace ClinicAppointments.Api.Features.Specializations;

public record SpecializationResponse
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }
}
