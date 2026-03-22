using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    public string GenerateAccessToken(User user)
    {
        throw new NotImplementedException();
    }

    public string GenerateRefreshToken()
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken> SaveRefreshTokenAsync(User user, string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(string token)
    {
        throw new NotImplementedException();
    }

    public Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
    {
        throw new NotImplementedException();
    }
}