
// backend/src/APT.Api/Controllers/ShortagesController.cs
using APT.Application.DTOs;
using APT.Application.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/shortages")]
public class ShortagesController : ControllerBase
{
    private readonly ShortageService _service;
    public ShortagesController(ShortageService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ShortageListItemDto>>> List([FromQuery] string? partNumber, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var list = await _service.ListAsync(partNumber, skip, take);
        return Ok(list);
    }
}
