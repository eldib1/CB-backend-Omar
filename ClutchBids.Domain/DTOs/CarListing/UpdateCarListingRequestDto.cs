namespace ClutchBids.Domain.DTOs.CarListing;

public class UpdateCarListingRequestDto
{
    public string ListingTitle { get; set; } = string.Empty;
    public string? ListingDescription { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public string Condition { get; set; } = string.Empty;
}
