
// backend/src/APT.Application/Mappings/ShortageProfile.cs
using AutoMapper;
using APT.Application.DTOs;
using APT.Domain.Entities;

namespace APT.Application.Mappings;

public class ShortageProfile : Profile
{
    public ShortageProfile()
    {
        CreateMap<Shortage, ShortageListItemDto>()
            .ForMember(d => d.PartNumber, opt => opt.MapFrom(s => s.Part.PartNumber));
    }
}
