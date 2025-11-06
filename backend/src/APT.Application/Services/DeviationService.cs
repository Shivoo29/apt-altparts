
// backend/src/APT.Application/Services/DeviationService.cs
using AutoMapper;
using AutoMapper.QueryableExtensions;
using APT.Application.DTOs;
using APT.Domain.Entities;
using APT.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace APT.Application.Services;

public class DeviationService
{
    private readonly AppDbContext _db;
    private readonly IMapper _mapper;

    public DeviationService(AppDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<List<DeviationListItemDto>> ListAsync(string? partNumber, int skip = 0, int take = 100)
    {
        var q = _db.Deviations.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(partNumber)) q = q.Where(d => d.Part.PartNumber.Contains(partNumber));
        return await q.OrderByDescending(d => d.EffectiveFrom)
            .Skip(skip).Take(take)
            .ProjectTo<DeviationListItemDto>(_mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<Deviation> CreateAsync(CreateDeviationRequest req, string approvedByUpn)
    {
        var deviation = _mapper.Map<Deviation>(req);
        deviation.ApprovedByUpn = approvedByUpn;
        _db.Deviations.Add(deviation);
        await _db.SaveChangesAsync();
        return deviation;
    }
}
