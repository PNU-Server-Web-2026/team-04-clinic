using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Doctors;

[ApiController]
[Route("api/doctors")]
public class DoctorsController(IDoctorService doctorService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DoctorResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var doctors = await doctorService.GetAllAsync(cancellationToken);
        return Ok(doctors);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DoctorResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.GetByIdAsync(id, cancellationToken);
        if (doctor == null)
        {
            return NotFound();
        }
        return Ok(doctor);
    }

    [HttpPost]
    public async Task<ActionResult<DoctorResponse>> Create(CreateDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = doctor.Id }, doctor);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateDoctorRequest request, CancellationToken cancellationToken)
    {
        var doctor = await doctorService.UpdateAsync(id, request, cancellationToken);
        if (doctor == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await doctorService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
