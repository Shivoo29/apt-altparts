
// backend/src/APT.Api/Controllers/PartsController.cs
using APT.Application.DTOs;
using APT.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/parts")]
public class PartsController : ControllerBase
{
    private readonly PartService _service;
    public PartsController(PartService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PartListItemDto>>> List([FromQuery] string? partNumber, [FromQuery] string? plant, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var list = await _service.ListAsync(partNumber, plant, skip, take);
        return Ok(list);
    }
}
