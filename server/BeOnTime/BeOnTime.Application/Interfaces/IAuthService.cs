using BeOnTime.Application.DTOs;

namespace BeOnTime.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequestDto registerRequestDto);
    Task<AuthResult> LoginASync(LoginRequestDto loginRequestDto);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
}