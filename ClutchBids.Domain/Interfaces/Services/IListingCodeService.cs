using ClutchBids.Domain.DTOs.ListingCode;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling listing codes
/// </summary>
public interface IListingCodeService
{
    Task<bool> GenerateCodesAsync(long adminUserId, int count);
    Task<IEnumerable<ListingCodeDto>> GetUnusedCodesAsync();
    Task<IEnumerable<ListingCodeDto>> GetAllCodesAsync();
    Task<bool> ValidateAndUseCodeAsync(string code, long listingId);
}
