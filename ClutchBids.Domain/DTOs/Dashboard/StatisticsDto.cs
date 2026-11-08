namespace ClutchBids.Domain.DTOs.Dashboard;

public class StatisticsDto
{
    public int TotalUsers { get; set; }
    public int NewUsersThisMonth { get; set; }
    public int ActiveListings { get; set; }
    public int NewListingsThisMonth { get; set; }
    public decimal AverageOfferPrice { get; set; }
    public int CompletedTransactions { get; set; }
    public int PendingApprovals { get; set; }
}
