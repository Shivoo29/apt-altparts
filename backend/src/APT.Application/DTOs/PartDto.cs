
// backend/src/APT.Application/DTOs/PartDto.cs
namespace APT.Application.DTOs;

public record PartListItemDto(int Id, string PartNumber, string? Description, string? Plant);
