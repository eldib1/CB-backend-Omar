using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class Notification : BaseEntity
{
    public long RecipientUserId { get; set; }
    public long? RelatedUserId { get; set; }
    public long? RelatedCarListingId { get; set; }
    public long? RelatedPriceOfferId { get; set; }
    public NotificationType NotificationType { get; set; }
    public string Title { get; set; }
    public string Message { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // Navigation properties
    public ApplicationUser RecipientUser { get; set; }
    public ApplicationUser RelatedUser { get; set; }
    public CarListing RelatedCarListing { get; set; }
    public PriceOffer RelatedPriceOffer { get; set; }
}
