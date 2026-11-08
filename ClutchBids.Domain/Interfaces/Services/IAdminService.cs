using ClutchBids.Domain.DTOs.Admin;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling admin user management operations
/// </summary>
public interface IAdminService
{
    Task<AdminUserManagementDto?> ApproveUserAsync(long userId, string approverEmail);
    Task<AdminUserManagementDto?> RejectUserAsync(long userId, string rejectionReason, string rejectorEmail);
    Task<IEnumerable<AdminUserManagementDto>> GetPendingVerificationUsersAsync();
    Task<IEnumerable<AdminUserManagementDto>> GetAllUsersAsync();
    Task<AdminUserManagementDto?> GetUserAsync(long userId);
    Task<bool> SuspendUserAsync(long userId);
    Task<bool> AssignListingCodesAsync(long userId, int count);
    Task<FileUploadResponseDto> UploadIdDocumentAsync(long userId, Stream fileStream, string fileName, string identityNumber, DateTime identityExpiryDate);
    Task<UserVerificationDto?> GetUserForVerificationAsync(long userId);
    Task<IEnumerable<UserVerificationDto>> GetPendingVerificationsAsync();
}
