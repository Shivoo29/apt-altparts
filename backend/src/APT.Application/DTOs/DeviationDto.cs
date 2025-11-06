
// backend/src/APT.Application/DTOs/DeviationDto.cs
namespace APT.Application.DTOs;

public record DeviationListItemDto(int Id, string PartNumber, string Type, DateTime EffectiveFrom, DateTime? EffectiveTo, string? ApprovedByUpn);
public record CreateDeviationRequest(int PartId, int? ProblemReportId, string Type, DateTime EffectiveFrom, DateTime? EffectiveTo);
