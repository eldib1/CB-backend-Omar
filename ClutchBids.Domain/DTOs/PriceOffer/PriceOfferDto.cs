namespace ClutchBids.Domain.DTOs.PriceOffer;

public class PriceOfferDto
{
    public long Id { get; set; }
    public long CarListingId { get; set; }
    public long BuyerId { get; set; }
    public string BuyerName { get; set; }
    public decimal OfferedPrice { get; set; }
    public string Message { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
}
