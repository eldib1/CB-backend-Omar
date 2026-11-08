using ClutchBids.Domain.DTOs.Auth;

namespace ClutchBids.Domain.Interfaces.Services;

/// <summary>
/// Service for handling user authentication operations
/// </summary>
public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<bool> VerifyEmailAsync(string email, string token);
    Task<bool> RequestPasswordResetAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
}
