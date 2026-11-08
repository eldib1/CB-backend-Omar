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
            .ForMember(dest => dest.Make, opt => opt.MapFrom(src => src.Car != null ? src.Car.Make : string.Empty))
            .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Car != null ? src.Car.Model : string.Empty))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Car != null ? src.Car.Year : 0))
            .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Car != null ? src.Car.Color : string.Empty))
            .ForMember(dest => dest.EngineSize, opt => opt.MapFrom(src => src.Car != null ? src.Car.EngineSize : string.Empty))
            .ForMember(dest => dest.FuelType, opt => opt.MapFrom(src => src.Car != null ? src.Car.FuelType : string.Empty))
            .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.Car != null ? src.Car.Transmission : string.Empty))
            .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Car != null ? (int)src.Car.Mileage : 0))
            .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Car != null ? src.Car.Condition : string.Empty))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src =>
                src.Images.Where(img => img.File != null).OrderBy(img => img.DisplayOrder).Select(img => img.File!.FileUrl).ToList()))
            .ForMember(dest => dest.OfferCount, opt => opt.MapFrom(src => 
                src.PriceOffers.Count));
    }
}
