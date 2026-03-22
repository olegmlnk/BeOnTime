using BeOnTime.Application.DTOs;
using Microsoft.AspNetCore.Authentication;

namespace BeOnTime.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterRequestDto registerRequestDto);
    Task<AuthResult> LoginASync(LoginRequestDto loginRequestDto);
    Task<AuthResult> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto);
}