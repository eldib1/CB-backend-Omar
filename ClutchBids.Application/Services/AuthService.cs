using System.Security.Claims;
using AutoMapper;
using ClutchBids.Domain.DTOs.Auth;
using ClutchBids.Domain.Entities;
using ClutchBids.Domain.Enums;
using ClutchBids.Domain.Interfaces.Managers;
using ClutchBids.Domain.Interfaces.Services;

namespace ClutchBids.Application.Services;

/// <summary>
/// Implementation of authentication service using Microsoft Identity framework
/// </summary>
public class AuthService : IAuthService
{
    private readonly IIdentityService _identityService;
    private readonly ITokenManager _tokenService;
    private readonly IEmailManager _emailService;
    private readonly IMapper _mapper;

    public AuthService(
        IIdentityService identityService,
        ITokenManager tokenService,
        IEmailManager emailService,
        IMapper mapper)
    {
        _identityService = identityService;
        _tokenService = tokenService;
        _emailService = emailService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _identityService.FindUserByEmailAsync(request.Email);
        
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        var passwordValid = await _identityService.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid email or password"
            };
        }

        if (!user.EmailConfirmed)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Your account is pending verification. Please check your email."
            };
        }

        if (user.Status == UserStatus.Suspended || user.Status == UserStatus.Rejected)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Your account has been suspended or rejected"
            };
        }

        var roles = await _identityService.GetUserRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, roles.ToList());
        var refreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponseDto

        {
            Success = true,
            Message = "Login successful",
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = _mapper.Map<UserDto>(user)
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Passwords do not match"
            };
        }

        var applicationUser = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            Suburb = request.Suburb,
            State = request.State,
            PostalCode = request.PostalCode,
            Status = UserStatus.PendingVerification,
            EmailConfirmed = false
        };

        var result = await _identityService.CreateUserAsync(applicationUser, request.Password);
        if (!result.Succeeded)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        // Assign User role
        var roleResult = await _identityService.AddUserToRoleAsync(applicationUser, "User");
        if (!roleResult.Succeeded)
        {
            // Role assignment failed but user was created, log this issue
            // For now, we'll continue with the registration
        }

        // Generate email confirmation token
        var confirmationToken = await _identityService.GenerateEmailConfirmationTokenAsync(applicationUser);

        // Send verification email
        await _emailService.SendEmailAsync(
            applicationUser.Email,
            "Verify Your ClutchBids Account",
            $"Please verify your email to activate your account");

        return new AuthResponseDto
        {
            Success = true,
            Message = "Registration successful. Please check your email to verify your account."
        };
    }

    public async Task<bool> VerifyEmailAsync(string email, string token)
    {
        var user = await _identityService.FindUserByEmailAsync(email);
        
        if (user == null)
            return false;

        var result = await _identityService.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return false;

        user.Status = UserStatus.Active;
        var updateResult = await _identityService.UpdateUserAsync(user);

        return updateResult.Succeeded;

    }

    public async Task<bool> RequestPasswordResetAsync(string email)
    {
        var user = await _identityService.FindUserByEmailAsync(email);
        
        if (user == null)
            return false;

        var resetToken = await _identityService.GeneratePasswordResetTokenAsync(user);

        await _emailService.SendEmailAsync(
            user.Email,
            "Reset Your Password",
            $"Click the link to reset your password");

        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await _identityService.FindUserByEmailAsync(email);
        
        if (user == null)
            return false;

        var result = await _identityService.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        // Validate the refresh token format
        if (!_tokenService.ValidateRefreshToken(request.RefreshToken))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid refresh token"
            };
        }

        // Extract claims from the expired access token
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid access token"
            };
        }


        // Get the user ID from the principal
        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier) ?? principal.FindFirst("sub");
        if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "Invalid token claims"
            };
        }

        // Fetch the user from the database to get current roles
        var user = await _identityService.FindUserByIdAsync(userId);
        if (user == null)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User not found"
            };
        }

        // Check if user is still active
        if (user.Status == UserStatus.Suspended || user.Status == UserStatus.Rejected)
        {
            return new AuthResponseDto
            {
                Success = false,
                Message = "User account is no longer active"
            };
        }

        // Generate new tokens
        var roles = await _identityService.GetUserRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email, roles.ToList());
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        return new AuthResponseDto
        {
            Success = true,
            Message = "Token refreshed successfully",
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            User = _mapper.Map<UserDto>(user)

        };
    }
}
