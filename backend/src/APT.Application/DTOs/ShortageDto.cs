
// backend/src/APT.Application/DTOs/ShortageDto.cs
namespace APT.Application.DTOs;

public record ShortageListItemDto(int Id, string PartNumber, DateTime Date, int QtyRequired, int QtyOnHand, int CoverageDays);
