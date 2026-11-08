namespace ClutchBids.Domain.DTOs.ListingCode;

public class GenerateListingCodesResponseDto
{
    public List<string> Codes { get; set; } = [];
    public bool Success { get; set; }
}
