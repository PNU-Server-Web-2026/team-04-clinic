using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Specializations;

[ApiController]
[Route("api/specializations")]
public sealed class SpecializationsController(
    ISpecializationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<SpecializationResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var specializations = await service.GetAllAsync(cancellationToken);

        return Ok(specializations);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<SpecializationResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var specialization = await service.GetByIdAsync(id, cancellationToken);

        if (specialization is null)
        {
            return NotFound();
        }

        return Ok(specialization);
    }

    [HttpPost]
    public async Task<ActionResult<SpecializationResponse>> Create(
        CreateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var specialization = await service.CreateAsync(request, cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = specialization.Id },
            specialization);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateSpecializationRequest request,
        CancellationToken cancellationToken)
    {
        var updated = await service.UpdateAsync(id, request, cancellationToken);

        if (!updated)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);

        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}