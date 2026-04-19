using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.Options;
using BeOnTime.Core.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace BeOnTime.Application.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly IRefreshTokenRepository _refreshTokens;

    public JwtTokenService(IOptions<JwtOptions> options, IRefreshTokenRepository refreshTokens)
    {
        _options = options.Value;
        _refreshTokens = refreshTokens;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpirationTimeInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public async Task<RefreshToken> SaveRefreshTokenAsync(User user, string refreshToken, string? userAgent)
    {
        var entity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshToken,
            UserId = user.Id,
            UserAgent = userAgent,
            Expires = DateTime.UtcNow.AddDays(_options.RefreshTokenExpirationDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        return await _refreshTokens.CreateAsync(entity);
    }

    public Task<RefreshToken?> GetRefreshTokenAsync(string token) =>
        _refreshTokens.GetByTokenAsync(token);

    public async Task RevokeRefreshTokenAsync(string token)
    {
        var existing = await _refreshTokens.GetByTokenAsync(token);
        if (existing is null) return;
        existing.IsRevoked = true;
        existing.UpdatedAt = DateTime.UtcNow;
        await _refreshTokens.UpdateAsync(existing);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken)
    {
        refreshToken.IsRevoked = true;
        refreshToken.UpdatedAt = DateTime.UtcNow;
        await _refreshTokens.UpdateAsync(refreshToken);
    }
}
