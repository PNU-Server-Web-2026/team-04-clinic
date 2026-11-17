using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Specializations;

[ApiController]
[Route("api/specializations")]
public class SpecializationsController(ISpecializationService specializationService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SpecializationResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var specializations = await specializationService.GetAllAsync(cancellationToken);
        return Ok(specializations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpecializationResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var specialization = await specializationService.GetByIdAsync(id, cancellationToken);
        if (specialization == null)
        {
            return NotFound();
        }
        return Ok(specialization);
    }

    [HttpPost]
    public async Task<ActionResult<SpecializationResponse>> Create(CreateSpecializationRequest request, CancellationToken cancellationToken)
    {
        var specialization = await specializationService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = specialization.Id }, specialization);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateSpecializationRequest request, CancellationToken cancellationToken)
    {
        var specialization = await specializationService.UpdateAsync(id, request, cancellationToken);
        if (specialization == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await specializationService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
