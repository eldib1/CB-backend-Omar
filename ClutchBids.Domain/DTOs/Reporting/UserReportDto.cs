namespace ClutchBids.Domain.DTOs.Reporting;

public class UserReportDto
{
    public long Id { get; set; }
    public long ReportedById { get; set; }
    public string ReportedByName { get; set; }
    public long ReportedUserId { get; set; }
    public string ReportedUserName { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string RelatedListingId { get; set; }
    public string RelatedChatSessionId { get; set; }
    public int Status { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string ReviewedByName { get; set; }
    public string AdminNotes { get; set; }
    public int? AdminAction { get; set; }
}
