using BeOnTime.Application.DTOs;

namespace BeOnTime.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequestDto registerRequestDto, string? userAgent);
    Task<AuthResult> LoginASync(LoginRequestDto loginRequestDto, string? userAgent);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto, string? userAgent);
}
