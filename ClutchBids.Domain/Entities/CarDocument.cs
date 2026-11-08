using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class CarDocument : BaseEntity
{
    public long CarListingId { get; set; }
    public long? FileId { get; set; }
    public DocumentType DocumentType { get; set; }
    public string Description { get; set; }

    // Navigation properties
    public CarListing CarListing { get; set; }
    public File? File { get; set; }
}
