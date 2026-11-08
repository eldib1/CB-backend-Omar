using AutoMapper;
using ClutchBids.Domain.DTOs.CarListing;
using ClutchBids.Domain.Entities;

namespace ClutchBids.Infrastructure.Mappings;

public class CarListingMappingProfile : Profile
{
    public CarListingMappingProfile()
    {
        CreateMap<CarListing, CarListingDto>()
            .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => 
                $"{src.Seller!.FirstName} {src.Seller.LastName}"))
            .ForMember(dest => dest.SellerLocation, opt => opt.MapFrom(src => 
                $"{src.Seller!.Suburb}, {src.Seller.State}"))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => 
                src.Images.Where(img => img.File != null).Select(img => img.File!.FileUrl).ToList()))
            .ForMember(dest => dest.OfferCount, opt => opt.MapFrom(src => 
                src.PriceOffers.Count));
    }
}
