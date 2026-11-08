using AutoMapper;
using ClutchBids.Domain.DTOs.PriceOffer;
using ClutchBids.Domain.Entities;

namespace ClutchBids.Infrastructure.Mappings;

public class PriceOfferMappingProfile : Profile
{
    public PriceOfferMappingProfile()
    {
        CreateMap<PriceOffer, PriceOfferDto>()
            .ForMember(dest => dest.BuyerName, opt => opt.MapFrom(src => 
                $"{src.Buyer!.FirstName} {src.Buyer.LastName}"));
    }
}
