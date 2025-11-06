
// backend/src/APT.Application/Mappings/ProblemReportProfile.cs
using AutoMapper;
using APT.Application.DTOs;
using APT.Domain.Entities;

namespace APT.Application.Mappings;

public class ProblemReportProfile : Profile
{
    public ProblemReportProfile()
    {
        CreateMap<ProblemReport, ProblemReportListItemDto>()
            .ForMember(d => d.PartNumber, opt => opt.MapFrom(s => s.Part.PartNumber));

        CreateMap<ProblemReport, ProblemReportDetailDto>()
            .ForMember(d => d.PartNumber, opt => opt.MapFrom(s => s.Part.PartNumber))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Part.Description))
            .ForMember(d => d.History, opt => opt.MapFrom(s => s.StatusHistory));

        CreateMap<PRStatusHistory, StatusHistoryItem>();
    }
}
