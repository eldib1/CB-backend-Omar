using ClutchBids.Domain.Enums;

namespace ClutchBids.Domain.Entities;

public class UserReport : BaseEntity
{
    public long ReportedById { get; set; }
    public long ReportedUserId { get; set; }
    public string Reason { get; set; }
    public string Description { get; set; }
    public long? RelatedListingId { get; set; }
    public long? RelatedChatSessionId { get; set; }
    public UserReportStatus Status { get; set; } // 0: Pending, 1: Under Review, 2: Resolved, 3: Dismissed
    public DateTime ReportedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public long? ReviewedById { get; set; }
    public string AdminNotes { get; set; }
    public UserReportAction? AdminAction { get; set; } // 0: Warning, 1: Suspension, 2: Ban, 3: No Action

    // Navigation properties
    public ApplicationUser ReportedBy { get; set; }
    public ApplicationUser ReportedUser { get; set; }
    public ApplicationUser ReviewedBy { get; set; }
    public CarListing RelatedListing { get; set; }
    public ChatSession RelatedChatSession { get; set; }
}
