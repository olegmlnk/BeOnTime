using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Task<RefreshToken> SaveRefreshTokenAsync(User user, string refreshToken);
    Task<RefreshToken?> GetRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(string token);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken);
}