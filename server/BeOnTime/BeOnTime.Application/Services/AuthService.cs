using BeOnTime.Application.DTOs;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;
using BeOnTime.Core.Enums;
using Microsoft.Extensions.Configuration;

namespace BeOnTime.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;
    
    public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
    }
    
    public async Task<AuthResult> RegisterAsync(RegisterRequestDto request)
    {
        if (await _userRepository.ExistsAsync(request.Email, request.Password))
            return AuthResult.Fail("Email already exists");
        
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
        
        //Generating token at this point
        var accessToken = _jwtTokenService.GenerateAccessToken(user);
        var refreshToken = _jwtTokenService.GenerateRefreshToken();
        await _jwtTokenService.SaveRefreshTokenAsync(user, refreshToken); 
        
        return AuthResult.Ok(new TokenResponseDto{
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiresAt = _configuration.GetValue<DateTime>("JwtOptions:ExpirationTimeInMinutes")
        });
    }

    public async Task<AuthResult> LoginASync(LoginRequestDto loginRequestDto)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResult> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        throw new NotImplementedException();
    }
}