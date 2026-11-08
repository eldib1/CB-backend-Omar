using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class PriceOffer : BaseEntity
{
    public long CarListingId { get; set; }
    public long BuyerId { get; set; }
    public decimal OfferedPrice { get; set; }
    public PriceOfferStatus Status { get; set; }
    public DateTime? AcceptedAt { get; set; }
    public DateTime? RejectedAt { get; set; }

    // Navigation properties
    public CarListing CarListing { get; set; }
    public ApplicationUser Buyer { get; set; }
}
