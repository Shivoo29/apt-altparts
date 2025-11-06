
// backend/src/APT.Application/Services/PartService.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using APT.Application.DTOs;
using APT.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APT.Application.Services;

public class PartService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public PartService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<PartListItemDto>> ListAsync(string? partNumber, string? plant, int skip = 0, int take = 100)
    {
        var q = _db.Parts.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(partNumber)) q = q.Where(p => p.PartNumber.Contains(partNumber));
        if (!string.IsNullOrWhiteSpace(plant)) q = q.Where(p => p.Plant == plant);
        return await q.OrderBy(p => p.PartNumber)
            .Skip(skip).Take(take)
            .ProjectTo<PartListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
