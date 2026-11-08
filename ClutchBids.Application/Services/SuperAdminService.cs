using AutoMapper;
using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.DTOs.Auth;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of super admin service for admin account management
/// </summary>
public class SuperAdminService : ISuperAdminService
{
    private readonly IIdentityService _identityService;
    private readonly IEmailManager _emailService;
    private readonly IMapper _mapper;

    public SuperAdminService(
        IIdentityService identityService,
        IEmailManager emailService,
        IMapper mapper)
    {
        _identityService = identityService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<UserDto?> CreateAdminAsync(CreateAdminRequestDto request)
    {
        // Check if email exists
        var existingUser = await _identityService.FindUserByEmailAsync(request.Email);
        if (existingUser != null)
            return null;

        var admin = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Status = UserStatus.Active,
            EmailConfirmed = true
        };

        var result = await _identityService.CreateUserAsync(admin, request.Password);
        if (!result.Succeeded)
            return null;

        // Assign Admin role
        var roleResult = await _identityService.AddUserToRoleAsync(admin, "Admin");
        if (!roleResult.Succeeded)
        {
            // Log this but continue
        }

        // Send welcome email
        await _emailService.SendEmailAsync(
            admin.Email,
            "Welcome to ClutchBids Admin Panel",
            $"Your admin account has been created. You can now log in with your credentials.");

        return _mapper.Map<UserDto>(admin);
    }

    public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
    {
        var allUsers = await _identityService.GetAllUsersAsync();
        var admins = new List<ApplicationUser>();

        foreach (var user in allUsers)
        {
            var roles = await _identityService.GetUserRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                admins.Add(user);
            }
        }

        return _mapper.Map<IEnumerable<UserDto>>(admins);
    }

    public async Task<bool> RemoveAdminAsync(long adminId)
    {
        var admin = await _identityService.FindUserByIdAsync(adminId);
        if (admin == null)
            return false;

        // Remove admin role
        var roleResult = await _identityService.RemoveUserFromRoleAsync(admin, "Admin");
        if (!roleResult.Succeeded)
            return false;

        return true;
    }
}
