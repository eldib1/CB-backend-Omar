using ClutchBids.Domain.DTOs.Admin;
using ClutchBids.Domain.DTOs.Auth;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling super admin operations (creating and managing admins)
/// </summary>
public interface ISuperAdminService
{
    Task<UserDto?> CreateAdminAsync(CreateAdminRequestDto request);
    Task<IEnumerable<UserDto>> GetAllAdminsAsync();
    Task<bool> RemoveAdminAsync(long adminId);
}
