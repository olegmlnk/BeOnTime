using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken?> GetByIdAsync(Guid userId, Guid id);
    Task<IReadOnlyList<RefreshToken>> GetActiveByUserIdAsync(Guid userId);
    Task UpdateAsync(RefreshToken token);
    Task<int> RevokeAllForUserAsync(Guid userId, string? exceptToken);
}
