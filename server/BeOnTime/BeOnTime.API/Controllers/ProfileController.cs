using System.Security.Claims;
using BeOnTime.Application.DTOs.Profile;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/profile")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IProfileService _profile;

    public ProfileController(IProfileService profile)
    {
        _profile = profile;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var dto = await _profile.GetAsync(userId);
        if (dto is null) return NotFound();
        return Ok(dto);
    }

    [HttpPut("username")]
    public async Task<IActionResult> UpdateUsername([FromBody] UpdateUsernameDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _profile.UpdateUsernameAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("email")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _profile.UpdateEmailAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _profile.ChangePasswordAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return Ok(result.Value);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _profile.DeleteAccountAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });
        return NoContent();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
