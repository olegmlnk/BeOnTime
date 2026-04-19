using System.Security.Claims;
using BeOnTime.Application.DTOs.Settings;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/settings")]
[Authorize]
public class SettingsController : ControllerBase
{
    private readonly IUserSettingsService _settings;

    public SettingsController(IUserSettingsService settings)
    {
        _settings = settings;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var dto = await _settings.GetAsync(userId);
        return Ok(dto);
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserSettingsDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var updated = await _settings.UpdateAsync(userId, dto);
        return Ok(updated);
    }

    [HttpGet("history")]
    public async Task<IActionResult> GetHistory()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var history = await _settings.GetHistoryAsync(userId);
        return Ok(history);
    }

    [HttpPost("history/{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var result = await _settings.RestoreFromHistoryAsync(userId, id);
        if (!result.Success)
            return NotFound(new { error = result.Error });
        return Ok(result.Value);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
