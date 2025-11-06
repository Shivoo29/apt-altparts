
// backend/src/APT.Api/Controllers/DeviationsController.cs
using APT.Application.DTOs;
using APT.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/deviations")]
public class DeviationsController : ControllerBase
{
    private readonly DeviationService _service;
    public DeviationsController(DeviationService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DeviationListItemDto>>> List([FromQuery] string? partNumber, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var list = await _service.ListAsync(partNumber, skip, take);
        return Ok(list);
    }

    [HttpPost]
    public async Task<ActionResult<DeviationListItemDto>> Create([FromBody] CreateDeviationRequest req)
    {
        var userUpn = User.Identity?.Name ?? "system@local";
        var deviation = await _service.CreateAsync(req, userUpn);
        return CreatedAtAction(nameof(List), new { id = deviation.Id }, deviation);
    }
}
