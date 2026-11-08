using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class CarListing : BaseEntity
{
    public long SellerId { get; set; }
    public long CarId { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? ReservePrice { get; set; }
    public string ListingTitle { get; set; }
    public string ListingDescription { get; set; }
    public CarListingStatus Status { get; set; }
    public DateTime? ListedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public long? ApproverId { get; set; }
    public DateTime? DeActivatedAt { get; set; }
    public long? DeActivatorId { get; set; }
    public DateTime? SoldAt { get; set; }
    public long? SoldTo { get; set; }
    public DateTime? ViewCount { get; set; }
    public bool RequiresValidation { get; set; }
    public string ListingCode { get; set; }
    public long? SaleProofFileId { get; set; }


    // Navigation properties
    public Car Car { get; set; }
    public ApplicationUser Seller { get; set; }
    public ApplicationUser Approver { get; set; }
    public ApplicationUser DeActivator { get; set; }
    public File SaleProofFile { get; set; }
    public ICollection<CarImage> Images { get; set; }
    public ICollection<CarDocument> Documents { get; set; }
    public ICollection<CarVideo> Videos { get; set; }
    public ICollection<PriceOffer> PriceOffers { get; set; }
    public ICollection<ChatSession> ChatSessions { get; set; }
}
