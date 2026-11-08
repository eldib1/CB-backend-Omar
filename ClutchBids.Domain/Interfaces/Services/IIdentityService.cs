using ClutchBids.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Identity service for managing users and roles using Microsoft Identity
/// </summary>
public interface IIdentityService
{
    // User Management
    Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
    Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
    Task<IdentityResult> DeleteUserAsync(ApplicationUser user);
    Task<ApplicationUser> FindUserByIdAsync(long userId);
    Task<ApplicationUser> FindUserByEmailAsync(string email);
    Task<ApplicationUser> FindUserByNameAsync(string userName);
    Task<IList<ApplicationUser>> GetAllUsersAsync();
    Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(ApplicationUser user, string currentPassword, string newPassword);
    Task<string> GeneratePasswordResetTokenAsync(ApplicationUser user);
    Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
    Task<string> GenerateEmailConfirmationTokenAsync(ApplicationUser user);
    Task<IdentityResult> ConfirmEmailAsync(ApplicationUser user, string token);

    // Role Management
    Task<IdentityResult> CreateRoleAsync(ApplicationRole role);
    Task<IdentityResult> UpdateRoleAsync(ApplicationRole role);
    Task<IdentityResult> DeleteRoleAsync(ApplicationRole role);
    Task<ApplicationRole> FindRoleByIdAsync(long roleId);
    Task<ApplicationRole> FindRoleByNameAsync(string roleName);
    Task<IList<ApplicationRole>> GetAllRolesAsync();

    // User-Role Assignment
    Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName);
    Task<IdentityResult> AddUserToRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);
    Task<IdentityResult> RemoveUserFromRoleAsync(ApplicationUser user, string roleName);
    Task<IdentityResult> RemoveUserFromRolesAsync(ApplicationUser user, IEnumerable<string> roleNames);
    Task<IList<string>> GetUserRolesAsync(ApplicationUser user);
    Task<bool> IsUserInRoleAsync(ApplicationUser user, string roleName);
    Task<IList<ApplicationUser>> GetUsersInRoleAsync(string roleName);
}
