namespace ClutchBids.Domain.DTOs.CarListing;

public class CreateCarListingRequestDto
{
    public string ListingTitle { get; set; } = string.Empty;
    public string? ListingDescription { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string VIN { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string BodyType { get; set; } = string.Empty;
    public string FuelType { get; set; } = string.Empty;
    public int Mileage { get; set; }
    public string Transmission { get; set; } = string.Empty;
    public string EngineSize { get; set; } = string.Empty;
    public string Condition { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ListingCodeValue { get; set; }
}
