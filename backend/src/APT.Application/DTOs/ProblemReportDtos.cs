// backend/src/APT.Application/DTOs/ProblemReportDtos.cs
using APT.Domain.Entities;

namespace APT.Application.DTOs;

public record ProblemReportListItemDto(
    int Id, string PartNumber, string ReasonCode, PRStatus Status, string? OwnerUpn, DateTime OpenedDate, DateTime? SlaDue);

public record ProblemReportDetailDto(
    int Id, string PartNumber, string? Description, string ReasonCode, PRStatus Status,
    string? Priority, string? OwnerUpn, DateTime OpenedDate, DateTime? ClosedDate, DateTime? SlaDue,
    IEnumerable<StatusHistoryItem> History);

public record StatusHistoryItem(DateTime ChangedAt, PRStatus NewStatus, string ChangedByUpn, string? Comments);

public record UpdateStatusRequest(PRStatus NewStatus, string? Comments);