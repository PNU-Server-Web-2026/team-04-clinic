using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.MedicalServices;

[ApiController]
[Route("api/medical-services")]
public class MedicalServicesController(IMedicalServiceService medicalServiceService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MedicalServiceResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var services = await medicalServiceService.GetAllAsync(cancellationToken);
        return Ok(services);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MedicalServiceResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var service = await medicalServiceService.GetByIdAsync(id, cancellationToken);
        if (service == null)
        {
            return NotFound();
        }
        return Ok(service);
    }

    [HttpPost]
    public async Task<ActionResult<MedicalServiceResponse>> Create(CreateMedicalServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await medicalServiceService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = service.Id }, service);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMedicalServiceRequest request, CancellationToken cancellationToken)
    {
        var service = await medicalServiceService.UpdateAsync(id, request, cancellationToken);
        if (service == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await medicalServiceService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
