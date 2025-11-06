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
    public async Task<ActionResult<IEnumerable<ProblemReportListItemDto>>> List([FromQuery] string? reasonCode, [FromQuery] PRStatus? status)
    {
        var list = await _service.ListAsync(reasonCode, status);
        var dto = list.Select(p => new ProblemReportListItemDto(
            p.Id, p.Part.PartNumber, p.ReasonCode, p.Status, p.OwnerUpn, p.OpenedDate, p.SlaDue));
        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProblemReportDetailDto>> Get(int id)
    {
        var pr = await _service.GetAsync(id);
        if (pr is null) return NotFound();

        var dto = new ProblemReportDetailDto(
            pr.Id, pr.Part.PartNumber, pr.Part.Description, pr.ReasonCode, pr.Status, pr.Priority, pr.OwnerUpn,
            pr.OpenedDate, pr.ClosedDate, pr.SlaDue,
            pr.StatusHistory.OrderBy(h => h.ChangedAt)
              .Select(h => new StatusHistoryItem(h.ChangedAt, h.NewStatus, h.ChangedByUpn, h.Comments)));

        return Ok(dto);
    }

    [HttpPatch("{id:int}/status")]
    public async Task<ActionResult> UpdateStatus(int id, [FromBody] UpdateStatusRequest req)
    {
        // TODO: replace with Entra ID user UPN
        var userUpn = User.Identity?.Name ?? "system@local";
        try
        {
            var ok = await _service.UpdateStatusAsync(id, req.NewStatus, userUpn, req.Comments);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}