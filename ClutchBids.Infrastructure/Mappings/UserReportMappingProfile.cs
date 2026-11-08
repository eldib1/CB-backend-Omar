using AutoMapper;
using ClutchBids.Domain.DTOs.Reporting;
using ClutchBids.Domain.Entities;

namespace ClutchBids.Infrastructure.Mappings;

public class UserReportMappingProfile : Profile
{
    public UserReportMappingProfile()
    {
        CreateMap<UserReport, UserReportDto>()
            .ForMember(dest => dest.ReportedByName, opt => opt.MapFrom(src => 
                $"{src.ReportedBy!.FirstName} {src.ReportedBy.LastName}"))
            .ForMember(dest => dest.ReportedUserName, opt => opt.MapFrom(src => 
                $"{src.ReportedUser!.FirstName} {src.ReportedUser.LastName}"))
            .ForMember(dest => dest.ReviewedByName, opt => opt.MapFrom(src => 
                src.ReviewedBy != null ? $"{src.ReviewedBy.FirstName} {src.ReviewedBy.LastName}" : null));

        CreateMap<CreateUserReportRequestDto, UserReport>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => (int)Domain.Enums.UserReportStatus.Pending))
            .ForMember(dest => dest.ReportedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
    }
}
