using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Info;

[ApiController]
[Route("api/info")]
public class InfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public ActionResult<InfoResponse> Get()
    {
        var assembly = typeof(Program).Assembly;
        var version = assembly.GetName().Version?.ToString()
            ?? throw new InvalidOperationException("The API assembly version could not be determined.");

        return Ok(new InfoResponse(
            "Clinic Appointment API",
            version,
            environment.EnvironmentName,
            DateTimeOffset.UtcNow));
    }
}
