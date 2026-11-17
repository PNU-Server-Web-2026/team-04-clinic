using Microsoft.AspNetCore.Mvc;

namespace ClinicAppointments.Api.Features.Cabinets;

[ApiController]
[Route("api/cabinets")]
public class CabinetsController(ICabinetService cabinetService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CabinetResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var cabinets = await cabinetService.GetAllAsync(cancellationToken);
        return Ok(cabinets);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CabinetResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var cabinet = await cabinetService.GetByIdAsync(id, cancellationToken);
        if (cabinet == null)
        {
            return NotFound();
        }
        return Ok(cabinet);
    }

    [HttpPost]
    public async Task<ActionResult<CabinetResponse>> Create(CreateCabinetRequest request, CancellationToken cancellationToken)
    {
        var cabinet = await cabinetService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = cabinet.Id }, cabinet);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateCabinetRequest request, CancellationToken cancellationToken)
    {
        var cabinet = await cabinetService.UpdateAsync(id, request, cancellationToken);
        if (cabinet == null)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await cabinetService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }
        return NoContent();
    }
}
