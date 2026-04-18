using System.Security.Claims;
using BeOnTime.Application.DTOs.Reminders;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/reminders")]
[Authorize]
public class RemindersController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public RemindersController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var reminders = await _reminderService.GetActiveAsync(userId);
        return Ok(reminders);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReminderDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _reminderService.CreateAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(result.Reminder);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateReminderDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _reminderService.UpdateAsync(userId, id, dto);
        if (result is null) return NotFound();
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(result.Reminder);
    }

    [HttpPatch("{id:guid}/dismiss")]
    public async Task<IActionResult> Dismiss(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var reminder = await _reminderService.DismissAsync(userId, id);
        return reminder is null ? NotFound() : Ok(reminder);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var deleted = await _reminderService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
