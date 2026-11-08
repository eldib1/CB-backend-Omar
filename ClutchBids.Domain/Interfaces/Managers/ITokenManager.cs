using System.Security.Claims;

namespace ClutchBids.Domain.Interfaces.Managers;

public interface ITokenManager
{
    string GenerateAccessToken(long userId, string email, List<string> roles);
    string GenerateRefreshToken();
    bool ValidateToken(string token);
    bool ValidateRefreshToken(string refreshToken);
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}
