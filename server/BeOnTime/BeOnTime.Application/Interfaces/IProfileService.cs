using BeOnTime.Application.DTOs.Profile;

namespace BeOnTime.Application.Interfaces;

public interface IProfileService
{
    Task<ProfileDto?> GetAsync(Guid userId);
    Task<ProfileResult<ProfileDto>> UpdateUsernameAsync(Guid userId, UpdateUsernameDto dto);
    Task<ProfileResult<ProfileDto>> UpdateEmailAsync(Guid userId, UpdateEmailDto dto);
    Task<ProfileResult<ProfileDto>> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<ProfileResult<ProfileDto>> DeleteAccountAsync(Guid userId, DeleteAccountDto dto);
}
