using ClutchBids.Domain.DTOs.Dashboard;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of dashboard service for retrieving system and admin statistics
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;

    public DashboardService(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<dynamic> GetDashboardStatisticsAsync()
    {
        var allUsers = await _identityService.GetAllUsersAsync();
        var activeUsers = allUsers.Where(u => u.Status == Domain.Enums.UserStatus.Active);
        var pendingUsers = allUsers.Where(u => u.Status == Domain.Enums.UserStatus.PendingVerification);
        var allListings = await _unitOfWork.CarListings.GetAllAsync();
        var activeListings = allListings.Where(l => l.Status == CarListingStatus.Active);
        var pendingListings = allListings.Where(l => l.Status == CarListingStatus.PendingApproval);
        var allOffers = await _unitOfWork.PriceOffers.GetAllAsync();

        var totalUsers = allUsers.Count();
        var activeUserCount = activeUsers.Count();
        var pendingUserCount = pendingUsers.Count();
        var totalListings = allListings.Count();
        var activeListingCount = activeListings.Count();
        var pendingListingCount = pendingListings.Count();
        var soldListings = allListings.Count(l => l.Status == CarListingStatus.Sold);
        var totalGrossAmount = allOffers.Where(o => o.Status == Domain.Enums.PriceOfferStatus.Accepted).Sum(o => o.OfferedPrice);
        var totalOffers = allOffers.Count();
        var pendingOffers = allOffers.Count(o => o.Status == Domain.Enums.PriceOfferStatus.Pending);
        var acceptedOffers = allOffers.Count(o => o.Status == Domain.Enums.PriceOfferStatus.Accepted);

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            ActiveUsers = activeUserCount,
            PendingVerificationUsers = pendingUserCount,
            TotalListings = totalListings,
            ActiveListings = activeListingCount,
            PendingApprovalListings = pendingListingCount,
            SoldListings = soldListings,
            TotalGrossAmount = totalGrossAmount,
            TotalOffers = totalOffers,
            PendingOffers = pendingOffers,
            AcceptedOffers = acceptedOffers
        };
    }

    public async Task<dynamic> GetSystemStatisticsAsync()
    {
        var allUsers = await _identityService.GetAllUsersAsync();
        var allListings = await _unitOfWork.CarListings.GetAllAsync();
        var activeListings = allListings.Where(l => l.Status == CarListingStatus.Active);
        var allOffers = await _unitOfWork.PriceOffers.GetAllAsync();

        var currentMonth = DateTime.UtcNow.Month;
        var currentYear = DateTime.UtcNow.Year;
        var newListingsThisMonth = activeListings.Count(l => l.ListedAt.HasValue && l.ListedAt.Value.Month == currentMonth && l.ListedAt.Value.Year == currentYear);
        var averageOfferPrice = allOffers.Any() ? allOffers.Average(o => o.OfferedPrice) : 0m;
        var completedTransactions = allOffers.Count(o => o.Status == Domain.Enums.PriceOfferStatus.Accepted);
        var pendingApprovals = allUsers.Count(u => u.Status == UserStatus.PendingVerification);

        return new StatisticsDto
        {
            TotalUsers = allUsers.Count(),
            NewUsersThisMonth = 0,  // Would need to track registration date separately
            ActiveListings = activeListings.Count(),
            NewListingsThisMonth = newListingsThisMonth,
            AverageOfferPrice = (decimal)averageOfferPrice,
            CompletedTransactions = completedTransactions,
            PendingApprovals = pendingApprovals
        };
    }
}
