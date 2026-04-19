using BeOnTime.Application.DTOs.Sessions;
using BeOnTime.Application.Interfaces;

namespace BeOnTime.Application.Services;

public class SessionService : ISessionService
{
    private readonly IRefreshTokenRepository _refreshTokens;

    public SessionService(IRefreshTokenRepository refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task<IReadOnlyList<SessionDto>> GetSessionsAsync(Guid userId, string? currentRefreshToken)
    {
        var tokens = await _refreshTokens.GetActiveByUserIdAsync(userId);
        return tokens.Select(t => new SessionDto
        {
            Id = t.Id,
            UserAgent = t.UserAgent,
            CreatedAt = t.CreatedAt,
            Expires = t.Expires,
            IsRevoked = t.IsRevoked,
            IsCurrent = currentRefreshToken != null && t.Token == currentRefreshToken
        }).ToList();
    }

    public async Task<bool> RevokeAsync(Guid userId, Guid sessionId)
    {
        var token = await _refreshTokens.GetByIdAsync(userId, sessionId);
        if (token is null) return false;

        if (!token.IsRevoked)
        {
            token.IsRevoked = true;
            token.UpdatedAt = DateTime.UtcNow;
            await _refreshTokens.UpdateAsync(token);
        }
        return true;
    }

    public Task<int> RevokeAllAsync(Guid userId, string? exceptRefreshToken) =>
        _refreshTokens.RevokeAllForUserAsync(userId, exceptRefreshToken);
}
