namespace ClinicAppointments.Api.Features.Specializations;

public sealed record CreateSpecializationRequest(
    string Name,
    string? Description);