namespace ClutchBids.Domain.Entities;

public class CarImage : BaseEntity
{
    public long CarListingId { get; set; }
    public long? FileId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }

    // Navigation properties
    public CarListing CarListing { get; set; }
    public File File { get; set; }
}
