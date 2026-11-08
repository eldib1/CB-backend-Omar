namespace ClutchBids.Domain.DTOs.CarListing;

public class CarListingDto
{
    public long Id { get; set; }
    public string ListingTitle { get; set; } = string.Empty;
    public string? ListingDescription { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public string EngineSize { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public string Transmission { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string Condition { get; set; } = string.Empty;
    public int Status { get; set; }
    public string SellerName { get; set; } = string.Empty;
    public string? SellerLocation { get; set; }
    public List<string> ImageUrls { get; set; } = [];
    public DateTime? ListedAt { get; set; }
    public int? OfferCount { get; set; }
    public bool IsFeatured { get; set; }
}
