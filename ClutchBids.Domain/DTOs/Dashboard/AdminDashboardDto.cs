namespace ClutchBids.Domain.DTOs.Dashboard;

public class AdminDashboardDto
{
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int PendingVerificationUsers { get; set; }
    public int TotalListings { get; set; }
    public int ActiveListings { get; set; }
    public int PendingApprovalListings { get; set; }
    public int SoldListings { get; set; }
    public decimal TotalGrossAmount { get; set; }
    public int TotalOffers { get; set; }
    public int PendingOffers { get; set; }
    public int AcceptedOffers { get; set; }
}
