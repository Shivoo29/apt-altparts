
// backend/src/APT.Api/Controllers/ProblemReportsController.cs
using APT.Application.DTOs;
using APT.Application.Services;
using APT.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/prs")]
public class ProblemReportsController : ControllerBase
{
    private readonly ProblemReportService _service;
    public ProblemReportsController(ProblemReportService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProblemReportListItemDto>>> List([FromQuery] string? reasonCode, [FromQuery] PRStatus? status, [FromQuery] int skip = 0, [FromQuery] int take = 100)
    {
        var list = await _service.ListAsync(reasonCode, status, skip, take);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProblemReportDetailDto>> Get(int id)
    {
        var pr = await _service.GetAsync(id);
        if (pr is null) return NotFound();
        return Ok(pr);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        // TODO: replace with Entra ID user UPN
        var userUpn = User.Identity?.Name ?? "system@local";
        var (success, error) = await _service.UpdateStatusAsync(id, req.NewStatus, userUpn, req.Comments);
        if (!success) return BadRequest(new { error });
        return NoContent();
    }
}
