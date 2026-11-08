using ClutchBids.Domain.DTOs.Reporting;

namespace ClutchBids.Domain.Interfaces.Services;

public interface IUserReportingService
{
    Task<UserReportDto?> ReportUserAsync(long reporterId, CreateUserReportRequestDto request);
    Task<IEnumerable<UserReportDto>> GetPendingReportsAsync();
    Task<IEnumerable<UserReportDto>> GetUserReportsAsync(long userId);
    Task<bool> ReviewReportAsync(long adminId, ReviewUserReportRequestDto request);
    Task<IEnumerable<UserReportDto>> GetReportsByStatusAsync(int status);
}
