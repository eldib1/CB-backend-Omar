namespace ClutchBids.Domain.DTOs.ListingCode;

public class ListingCodeDto
{
    public long Id { get; set; }
    public string Code { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
}
