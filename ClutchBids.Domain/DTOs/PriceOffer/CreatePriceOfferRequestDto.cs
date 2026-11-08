namespace ClutchBids.Domain.DTOs.PriceOffer;

public class CreatePriceOfferRequestDto
{
    public long CarListingId { get; set; }
    public decimal OfferedPrice { get; set; }
}
