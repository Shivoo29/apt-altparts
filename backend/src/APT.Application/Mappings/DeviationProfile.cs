
// backend/src/APT.Application/Mappings/DeviationProfile.cs
using AutoMapper;
using APT.Application.DTOs;
using APT.Domain.Entities;

namespace APT.Application.Mappings;

public class DeviationProfile : Profile
{
    public DeviationProfile()
    {
        CreateMap<Deviation, DeviationListItemDto>()
            .ForMember(d => d.PartNumber, opt => opt.MapFrom(s => s.Part.PartNumber));
        CreateMap<CreateDeviationRequest, Deviation>();
    }
}
