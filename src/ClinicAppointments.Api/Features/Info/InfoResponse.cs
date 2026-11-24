namespace ClinicAppointments.Api.Features.Info;

public sealed record InfoResponse(
    string Name,
    string Version,
    string Environment,
    DateTimeOffset Utc);
