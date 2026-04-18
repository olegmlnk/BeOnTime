using BeOnTime.Application.DTOs;
using BeOnTime.Application.Interfaces;
using BeOnTime.Application.Options;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using Microsoft.Extensions.Options;

namespace BeOnTime.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IOptions<JwtOptions> jwtOptions)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthResult> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.ExistsAsync(request.Email, request.UserName))
            return AuthResult.Fail("User with this email or username already exists");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = passwordHash,
            Role = Role.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _userRepository.CreateAsync(user);

        return AuthResult.Ok(await IssueTokensAsync(user));
    }

    public async Task<AuthResult> LoginASync(LoginRequestDto loginRequestDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginRequestDto.Email);
        if (user is null)
            return AuthResult.Fail("Invalid credentials");

        if (!BCrypt.Net.BCrypt.Verify(loginRequestDto.Password, user.PasswordHash))
            return AuthResult.Fail("Invalid credentials");

        return AuthResult.Ok(await IssueTokensAsync(user));
    }

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var existing = await _jwtTokenService.GetRefreshTokenAsync(refreshTokenRequestDto.RefreshToken);
        if (existing is null || existing.IsRevoked || existing.Expires < DateTime.UtcNow)
            return AuthResult.Fail("Invalid or expired refresh token");

        var user = await _userRepository.GetByIdAsync(existing.UserId);
        if (user is null)
            return AuthResult.Fail("User not found");

        await _jwtTokenService.RevokeRefreshTokenAsync(existing);

        return AuthResult.Ok(await IssueTokensAsync(user));
    }

    private async Task<TokenResponseDto> IssueTokensAsync(User user)
    {
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        await _jwtTokenService.SaveRefreshTokenAsync(user, refreshToken);

        return new TokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtOptions.ExpirationTimeInMinutes)
        };
    }
}
