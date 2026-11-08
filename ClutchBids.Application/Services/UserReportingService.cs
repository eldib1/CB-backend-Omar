using AutoMapper;
using ClutchBids.Domain.DTOs.Reporting;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

public class UserReportingService : IUserReportingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IMapper _mapper;

    public UserReportingService(IUnitOfWork unitOfWork, IIdentityService identityService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _mapper = mapper;
    }

    public async Task<UserReportDto?> ReportUserAsync(long reporterId, CreateUserReportRequestDto request)
    {
        // Validate that both users exist
        var reporter = await _identityService.FindUserByIdAsync(reporterId);
        var reportedUser = await _identityService.FindUserByIdAsync(request.ReportedUserId);

        if (reporter == null || reportedUser == null)
            return null;

        // Check if reporter and reported user are different
        if (reporterId == request.ReportedUserId)
            return null;

        // Create the report
        var report = new UserReport
        {
            ReportedById = reporterId,
            ReportedUserId = request.ReportedUserId,
            Reason = request.Reason,
            Description = request.Description,
            RelatedListingId = request.RelatedListingId,
            RelatedChatSessionId = request.RelatedChatSessionId,
            Status = (int)UserReportStatus.Pending,
            ReportedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserReports.AddAsync(report);
        await _unitOfWork.SaveChangesAsync();

        // Fetch the created report with related data and map to DTO
        var createdReport = await _unitOfWork.UserReports.GetByIdAsync(report.Id);
        if (createdReport != null)
        {
            // Manually load navigation properties since EF might not have loaded them
            var fullReport = await _unitOfWork.UserReports.GetAllAsync();
            createdReport = fullReport.FirstOrDefault(r => r.Id == report.Id);
        }

        return createdReport != null ? _mapper.Map<UserReportDto>(createdReport) : null;
    }

    public async Task<IEnumerable<UserReportDto>> GetPendingReportsAsync()
    {
        var reports = await GetPendingReportsFromDbAsync();
        return _mapper.Map<IEnumerable<UserReportDto>>(reports);
    }

    public async Task<IEnumerable<UserReportDto>> GetUserReportsAsync(long userId)
    {
        var reports = await GetReportsByUserFromDbAsync(userId);
        return _mapper.Map<IEnumerable<UserReportDto>>(reports);
    }

    public async Task<bool> ReviewReportAsync(long adminId, ReviewUserReportRequestDto request)
    {
        var report = await _unitOfWork.UserReports.GetByIdAsync(request.ReportId);
        if (report == null)
            return false;

        // Verify admin exists
        var admin = await _identityService.FindUserByIdAsync(adminId);
        if (admin == null)
            return false;

        report.Status = (UserReportStatus)request.Status;
        report.AdminNotes = request.AdminNotes;
        report.AdminAction = request.AdminAction.HasValue ? (UserReportAction)request.AdminAction.Value : null;
        report.ReviewedAt = DateTime.UtcNow;
        report.ReviewedById = adminId;

        // If action is taken, update the reported user's status accordingly
        if (request.AdminAction.HasValue && request.AdminAction != (int)UserReportAction.NoAction)
        {
            var reportedUser = await _identityService.FindUserByIdAsync(report.ReportedUserId);
            if (reportedUser != null)
            {
                switch ((UserReportAction)request.AdminAction.Value)
                {
                    case UserReportAction.Warning:
                        // Send warning notification to user
                        break;
                    case UserReportAction.Suspension:
                        reportedUser.Status = UserStatus.Suspended;
                        break;
                    case UserReportAction.Ban:
                        reportedUser.Status = UserStatus.Banned;
                        break;
                }

                await _identityService.UpdateUserAsync(reportedUser);
            }
        }

        await _unitOfWork.UserReports.UpdateAsync(report);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserReportDto>> GetReportsByStatusAsync(int status)
    {
        var reports = await GetReportsByStatusFromDbAsync(status);
        return _mapper.Map<IEnumerable<UserReportDto>>(reports);
    }

    private async Task<IEnumerable<UserReport>> GetPendingReportsFromDbAsync()
    {
        var allReports = await _unitOfWork.UserReports.GetAllAsync();
        return allReports
            .Where(r => r.Status == (int)UserReportStatus.Pending)
            .OrderByDescending(r => r.ReportedAt)
            .ToList();
    }

    private async Task<IEnumerable<UserReport>> GetReportsByUserFromDbAsync(long userId)
    {
        var allReports = await _unitOfWork.UserReports.GetAllAsync();
        return allReports
            .Where(r => r.ReportedUserId == userId)
            .OrderByDescending(r => r.ReportedAt)
            .ToList();
    }

    private async Task<IEnumerable<UserReport>> GetReportsByStatusFromDbAsync(int status)
    {
        var allReports = await _unitOfWork.UserReports.GetAllAsync();
        return allReports
            .Where(r => r.Status == (UserReportStatus)status)
            .OrderByDescending(r => r.ReportedAt)
            .ToList();
    }
}
