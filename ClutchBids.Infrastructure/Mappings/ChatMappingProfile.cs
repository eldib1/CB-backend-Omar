using AutoMapper;
using ClutchBids.Domain.DTOs.Chat;
using ClutchBids.Domain.Entities;

namespace ClutchBids.Infrastructure.Mappings;

public class ChatMappingProfile : Profile
{
    public ChatMappingProfile()
    {
        CreateMap<ChatMessage, ChatMessageDto>()
            .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src =>
                $"{src.Sender!.FirstName} {src.Sender.LastName}"));

        CreateMap<ChatSession, ChatSessionDto>()
            .ForMember(dest => dest.FirstUserName, opt => opt.MapFrom(src =>
                $"{src.FirstUser!.FirstName} {src.FirstUser.LastName}"))
            .ForMember(dest => dest.SecondUserName, opt => opt.MapFrom(src =>
                $"{src.SecondUser!.FirstName} {src.SecondUser.LastName}"))
            .ForMember(dest => dest.CarTitle, opt => opt.MapFrom(src =>
                src.CarListing != null ? src.CarListing.ListingTitle : null));
    }
}
