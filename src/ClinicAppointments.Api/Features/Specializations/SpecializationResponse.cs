namespace ClinicAppointments.Api.Features.Specializations;

public sealed record SpecializationResponse(
    int Id,
    string Name,
    string? Description);