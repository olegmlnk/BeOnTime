using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> CreateAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task UpdateAsync(RefreshToken token);
}
