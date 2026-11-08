using ClutchBids.Domain.DTOs.CarListing;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling car listing operations
/// </summary>
public interface ICarListingService
{
    Task<CarListingDto?> CreateListingAsync(long sellerId, CreateCarListingRequestDto request);
    Task<CarListingDto?> UpdateListingAsync(long listingId, long sellerId, UpdateCarListingRequestDto request);
    Task<bool> DeleteListingAsync(long listingId, long sellerId);
    Task<CarListingDto?> GetListingAsync(long listingId);
    Task<IEnumerable<CarListingDto>> GetSellerListingsAsync(long sellerId);
    Task<IEnumerable<CarListingDto>> GetActiveListingsAsync();
    Task<dynamic> SearchListingsAsync(CarListingSearchDto searchDto);
    Task<dynamic?> GetListingDetailsAsync(long id, bool isAuthenticated);
    Task<bool> SubmitListingForApprovalAsync(long id, long userId);
    Task<bool> ApproveListingAsync(long id, long userId);
    Task<bool> RejectListingAsync(long id, long userId, string reason);
    Task<bool> DeactivateListingAsync(long id, long userId);
    Task<bool> MarkListingAsSoldAsync(long id, long userId);
    Task<bool> UploadSaleProofAsync(long id, long userId, Stream stream, string fileName);
}
