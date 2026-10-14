namespace ClinicAppointments.Api.Features.Specializations;

public sealed record UpdateSpecializationRequest(
    string Name,
    string? Description);