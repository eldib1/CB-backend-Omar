namespace ClutchBids.Domain.DTOs.Reporting;

public class CreateUserReportRequestDto
{
    public long ReportedUserId { get; set; }
    public string Reason { get; set; }
    public string Description { get; set; }
    public long? RelatedListingId { get; set; }
    public long? RelatedChatSessionId { get; set; }
}
