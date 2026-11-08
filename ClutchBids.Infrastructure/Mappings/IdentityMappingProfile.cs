using AutoMapper;
using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.DTOs.Auth;
using ClutchBids.Domain.Entities;

namespace ClutchBids.Infrastructure.Mappings;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
        {
            // Map ApplicationUser to UserDto
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.EmailConfirmed))
                .ReverseMap();


        // Map ApplicationUser to AdminUserManagementDto
        CreateMap<ApplicationUser, AdminUserManagementDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.ApprovedBy, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IdDocumentUrl, opt => opt.Ignore())
            .ForMember(dest => dest.Roles, opt => opt.Ignore());


        // Map ApplicationUser to UserVerificationDto
        CreateMap<ApplicationUser, UserVerificationDto>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.IdDocumentUrl, opt => opt.Ignore());


        // Map CreateUserRequest to ApplicationUser
        CreateMap<RegisterRequestDto, ApplicationUser>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}
