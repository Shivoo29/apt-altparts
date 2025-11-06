
// backend/src/APT.Application/Mappings/PartProfile.cs
using AutoMapper;
using APT.Application.DTOs;
using APT.Domain.Entities;

namespace APT.Application.Mappings;

public class PartProfile : Profile
{
    public PartProfile()
    {
        CreateMap<Part, PartListItemDto>();
    }
}
