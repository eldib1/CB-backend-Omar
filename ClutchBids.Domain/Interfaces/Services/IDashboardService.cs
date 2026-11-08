namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling dashboard statistics
/// </summary>
public interface IDashboardService
{
    Task<dynamic> GetDashboardStatisticsAsync();
    Task<dynamic> GetSystemStatisticsAsync();
}
