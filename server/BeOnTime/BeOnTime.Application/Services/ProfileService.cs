using BeOnTime.Application.DTOs.Profile;
using BeOnTime.Application.Interfaces;
using BeOnTime.Core.Entities;

namespace BeOnTime.Application.Services;

public class ProfileService : IProfileService
{
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshTokens;

    public ProfileService(IUserRepository users, IRefreshTokenRepository refreshTokens)
    {
        _users = users;
        _refreshTokens = refreshTokens;
    }

    public async Task<ProfileDto?> GetAsync(Guid userId)
    {
        var user = await _users.GetByIdAsync(userId);
        return user is null ? null : Map(user);
    }

    public async Task<ProfileResult<ProfileDto>> UpdateUsernameAsync(Guid userId, UpdateUsernameDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.UserName) || dto.UserName.Length < 3)
            return ProfileResult<ProfileDto>.Fail("Username must be at least 3 characters");

        var user = await _users.GetByIdAsync(userId);
        if (user is null)
            return ProfileResult<ProfileDto>.Fail("User not found");

        if (await _users.ExistsExcludingAsync(user.Email, dto.UserName, userId))
            return ProfileResult<ProfileDto>.Fail("Username already taken");

        user.UserName = dto.UserName;
        user.UpdatedAt = DateTime.UtcNow;
        await _users.UpdateAsync(user);

        return ProfileResult<ProfileDto>.Ok(Map(user));
    }

    public async Task<ProfileResult<ProfileDto>> UpdateEmailAsync(Guid userId, UpdateEmailDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return ProfileResult<ProfileDto>.Fail("Email is required");

        var user = await _users.GetByIdAsync(userId);
        if (user is null)
            return ProfileResult<ProfileDto>.Fail("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ProfileResult<ProfileDto>.Fail("Invalid current password");

        if (await _users.ExistsExcludingAsync(dto.Email, user.UserName, userId))
            return ProfileResult<ProfileDto>.Fail("Email already taken");

        user.Email = dto.Email;
        user.UpdatedAt = DateTime.UtcNow;
        await _users.UpdateAsync(user);

        return ProfileResult<ProfileDto>.Ok(Map(user));
    }

    public async Task<ProfileResult<ProfileDto>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        if (dto.NewPassword != dto.ConfirmNewPassword)
            return ProfileResult<ProfileDto>.Fail("New password and confirmation do not match");

        if (string.IsNullOrWhiteSpace(dto.NewPassword) || dto.NewPassword.Length < 8)
            return ProfileResult<ProfileDto>.Fail("New password must be at least 8 characters");

        var user = await _users.GetByIdAsync(userId);
        if (user is null)
            return ProfileResult<ProfileDto>.Fail("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ProfileResult<ProfileDto>.Fail("Invalid current password");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        await _users.UpdateAsync(user);

        await _refreshTokens.RevokeAllForUserAsync(userId, null);

        return ProfileResult<ProfileDto>.Ok(Map(user));
    }

    public async Task<ProfileResult<ProfileDto>> DeleteAccountAsync(Guid userId, DeleteAccountDto dto)
    {
        var user = await _users.GetByIdAsync(userId);
        if (user is null)
            return ProfileResult<ProfileDto>.Fail("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            return ProfileResult<ProfileDto>.Fail("Invalid current password");

        var now = DateTime.UtcNow;
        user.DeletedAt = now;
        user.UpdatedAt = now;
        await _users.UpdateAsync(user);

        await _refreshTokens.RevokeAllForUserAsync(userId, null);

        return ProfileResult<ProfileDto>.Ok(Map(user));
    }

    private static ProfileDto Map(User user) => new()
    {
        Id = user.Id,
        UserName = user.UserName,
        Email = user.Email,
        Role = user.Role,
        CreatedAt = user.CreatedAt
    };
}
