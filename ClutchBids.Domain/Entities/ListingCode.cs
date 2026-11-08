namespace ClutchBids.Domain.Entities;

public class ListingCode : BaseEntity
{
    public string Code { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    public long? UsedByListingId { get; set; }
    public long CreatedByUserId { get; set; }

    // Navigation properties
    public ApplicationUser CreatedByUser { get; set; }
    public CarListing UsedByListing { get; set; }
}
