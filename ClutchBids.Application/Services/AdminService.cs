using System.Text;
using AutoMapper;
using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Repositories;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of admin service for handling user management, approval operations, and user verification
/// </summary>
public class AdminService : IAdminService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIdentityService _identityService;
    private readonly IEmailManager _emailService;
    private readonly INotificationManager _notificationService;
    private readonly IFileUploadManager _fileUploadService;
    private readonly IMapper _mapper;

    public AdminService(
        IUnitOfWork unitOfWork,
        IIdentityService identityService,
        IEmailManager emailService,
        INotificationManager notificationService,
        IFileUploadManager fileUploadService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
        _emailService = emailService;
        _notificationService = notificationService;
        _fileUploadService = fileUploadService;
        _mapper = mapper;
    }

    public async Task<AdminUserManagementDto?> ApproveUserAsync(long userId, string approverEmail)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null || user.Status != UserStatus.PendingVerification)
            return null;

        var approver = await _identityService.FindUserByEmailAsync(approverEmail);
        
        user.Status = UserStatus.Active;
        user.ApprovedAt = DateTime.UtcNow;
        user.ApprovedBy = approver?.Id;

        await _identityService.UpdateUserAsync(user);

        // Send approval notification
        await _emailService.SendUserApprovalEmailAsync(user.Email, user.FirstName);
        await _notificationService.SendUserApprovalNotificationAsync(userId, approverEmail);

        return _mapper.Map<AdminUserManagementDto>(user);
    }

    public async Task<AdminUserManagementDto?> RejectUserAsync(long userId, string rejectionReason, string rejectorEmail)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null || user.Status != UserStatus.PendingVerification)
            return null;

        user.Status = UserStatus.Rejected;
        user.RejectionReason = rejectionReason;

        await _identityService.UpdateUserAsync(user);

        // Send rejection notification
        await _emailService.SendUserRejectionEmailAsync(user.Email, user.FirstName, rejectionReason);
        await _notificationService.SendUserRejectionNotificationAsync(userId, rejectionReason);

        return _mapper.Map<AdminUserManagementDto>(user);
    }

    public async Task<IEnumerable<AdminUserManagementDto>> GetPendingVerificationUsersAsync()
    {
        var users = await _identityService.GetAllUsersAsync();
        var pendingUsers = users.Where(u => u.Status == UserStatus.PendingVerification);
        return _mapper.Map<IEnumerable<AdminUserManagementDto>>(pendingUsers.ToList());
    }

    public async Task<IEnumerable<AdminUserManagementDto>> GetAllUsersAsync()
    {
        var users = await _identityService.GetAllUsersAsync();
        return _mapper.Map<IEnumerable<AdminUserManagementDto>>(users);
    }

    public async Task<AdminUserManagementDto?> GetUserAsync(long userId)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return null;

        return _mapper.Map<AdminUserManagementDto>(user);
    }

    public async Task<bool> SuspendUserAsync(long userId)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return false;

        user.Status = UserStatus.Suspended;
        await _identityService.UpdateUserAsync(user);

        return true;
    }

    public async Task<bool> AssignListingCodesAsync(long userId, int count)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
            return false;

        var codes = new List<ListingCode>();
        for (int i = 0; i < count; i++)
        {
            codes.Add(new ListingCode
            {
                Code = GenerateListingCode(),
                IsUsed = false,
                CreatedByUserId = userId
            });
        }

        await _unitOfWork.ListingCodes.AddRangeAsync(codes);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserVerificationDto>> GetPendingVerificationsAsync()
    {
        var users = await _identityService.GetAllUsersAsync();
        var pendingUsers = users.Where(u => u.Status == UserStatus.PendingVerification);
        return _mapper.Map<IEnumerable<UserVerificationDto>>(pendingUsers.ToList());
    }

    public async Task<UserVerificationDto?> GetUserForVerificationAsync(long userId)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null || user.Status != UserStatus.PendingVerification)
            return null;

        return _mapper.Map<UserVerificationDto>(user);
    }

    public async Task<FileUploadResponseDto> UploadIdDocumentAsync(
        long userId,
        Stream documentStream,
        string fileName,
        string identityNumber,
        DateTime identityExpiryDate)
    {
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
        {
            return new FileUploadResponseDto
            {
                Success = false,
                Message = "User not found"
            };
        }

        try
        {
            var filePath = await _fileUploadService.UploadIdDocumentAsync(documentStream, fileName);

            if (string.IsNullOrEmpty(filePath))
            {
                return new FileUploadResponseDto
                {
                    Success = false,
                    Message = "Failed to upload document"
                };
            }

            // Create a new File entity
            var file = new Domain.Entities.File
            {
                FileName = fileName,
                FileUrl = filePath,
                FileType = FileType.Document,
                MimeType = "application/pdf",
                FileSizeInBytes = documentStream.Length,
                UploadedById = userId
            };

            await _unitOfWork.Files.AddAsync(file);
            await _unitOfWork.SaveChangesAsync();

            user.IdDocumentFileId = file.Id;
            user.IdentityNumber = identityNumber;
            user.IdentityExpiryDate = identityExpiryDate;

            await _identityService.UpdateUserAsync(user);

            return new FileUploadResponseDto
            {
                Success = true,
                FilePath = filePath,
                Message = "Document uploaded successfully"
            };
        }
        catch (Exception ex)
        {
            return new FileUploadResponseDto
            {
                Success = false,
                Message = "Error uploading document"
            };
        }
    }

    private static string GenerateListingCode()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var random = new Random();
        var code = new StringBuilder();
        for (int i = 0; i < 8; i++)
        {
            code.Append(chars[random.Next(chars.Length)]);
        }
        return code.ToString();
    }
}
