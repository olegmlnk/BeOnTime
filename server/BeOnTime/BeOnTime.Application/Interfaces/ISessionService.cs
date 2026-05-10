using BeOnTime.Application.DTOs.Sessions;

namespace BeOnTime.Application.Interfaces;

public interface ISessionService
{
    Task<IReadOnlyList<SessionDto>> GetSessionsAsync(Guid userId, string? currentRefreshToken);
    Task<bool> RevokeAsync(Guid userId, Guid sessionId);
    Task<int> RevokeAllAsync(Guid userId, string? exceptRefreshToken);
}
