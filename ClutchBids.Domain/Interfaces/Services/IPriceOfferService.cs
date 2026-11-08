using ClutchBids.Domain.DTOs.PriceOffer;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling price offers on car listings
/// </summary>
public interface IPriceOfferService
{
    Task<PriceOfferDto?> CreateOfferAsync(long buyerId, CreatePriceOfferRequestDto request);
    Task<PriceOfferDto?> AcceptOfferAsync(long offerId, long sellerId);
    Task<PriceOfferDto?> RejectOfferAsync(long offerId, long sellerId, string rejectionReason);
    Task<IEnumerable<PriceOfferDto>> GetOffersByListingAsync(long listingId);
    Task<IEnumerable<PriceOfferDto>> GetOffersByBuyerAsync(long buyerId);
    Task<IEnumerable<PriceOfferDto>> GetOffersBySellerAsync(long sellerId);
    Task<PriceOfferDto?> GetOfferAsync(long offerId);
    Task<bool> DeleteOfferAsync(long offerId, long buyerId);
}
