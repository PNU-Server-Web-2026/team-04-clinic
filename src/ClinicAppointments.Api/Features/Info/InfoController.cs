using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Info;

[ApiController]
[Route("api/info")]
public class InfoController(IHostEnvironment environment) : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var assembly = Assembly.GetEntryAssembly()
            ?? throw new InvalidOperationException("The entry assembly could not be determined.");
        var version = assembly.GetName().Version?.ToString()
            ?? throw new InvalidOperationException("The entry assembly version could not be determined.");

        return Ok(new
        {
            name = "Clinic Appointment API",
            version,
            environment = environment.EnvironmentName,
            utc = DateTimeOffset.UtcNow
        });
    }
}
