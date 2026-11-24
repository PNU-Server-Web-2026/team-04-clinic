using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Patients;

[ApiController]
[Route("api/patients")]
public class PatientsController(IPatientService patientService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var patients = await patientService.GetAllAsync(cancellationToken);
        return Ok(patients);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var patient = await patientService.GetByIdAsync(id, cancellationToken);
        if (patient == null)
        {
            return NotFound();
        }
        return Ok(patient);
    }

    [HttpPost]
    public async Task<ActionResult<PatientResponse>> Create(CreatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = await patientService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdatePatientRequest request, CancellationToken cancellationToken)
    {
        var patient = await patientService.UpdateAsync(id, request, cancellationToken);
        if (patient == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await patientService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
