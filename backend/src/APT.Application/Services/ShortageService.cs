
// backend/src/APT.Application/Services/ShortageService.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using APT.Application.DTOs;
using APT.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APT.Application.Services;

public class ShortageService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public ShortageService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<ShortageListItemDto>> ListAsync(string? partNumber, int skip = 0, int take = 100)
    {
        var q = _db.Shortages.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(partNumber)) q = q.Where(s => s.Part.PartNumber.Contains(partNumber));
        return await q.OrderByDescending(s => s.Date)
            .Skip(skip).Take(take)
            .ProjectTo<ShortageListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }
}
