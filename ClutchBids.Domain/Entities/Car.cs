namespace ClutchBids.Domain.Entities;

public class Car : BaseEntity
{
    public long CarListingId { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public string Color { get; set; }
    public string VIN { get; set; }
    public string RegistrationNumber { get; set; }
    public long Mileage { get; set; }
    public string Transmission { get; set; }
    public string FuelType { get; set; }
    public string BodyType { get; set; }
    public string EngineSize { get; set; }
    public string Condition { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public CarListing CarListing { get; set; }
}
