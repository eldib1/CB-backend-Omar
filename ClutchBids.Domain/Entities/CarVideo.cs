namespace ClutchBids.Domain.Entities;

public class CarVideo : BaseEntity
{
    public long CarListingId { get; set; }
    public long? FileId { get; set; }
    public string Description { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public CarListing CarListing { get; set; }
    public File? File { get; set; }
}
