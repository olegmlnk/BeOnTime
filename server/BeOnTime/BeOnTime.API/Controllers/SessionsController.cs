using System.Security.Claims;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/sessions")]
[Authorize]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessions;

    public SessionsController(ISessionService sessions)
    {
        _sessions = sessions;
    }

    public class RevokeAllRequest
    {
        public string? ExceptRefreshToken { get; set; }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? current)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var items = await _sessions.GetSessionsAsync(userId, current);
        return Ok(items);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Revoke(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var ok = await _sessions.RevokeAsync(userId, id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("revoke-all")]
    public async Task<IActionResult> RevokeAll([FromBody] RevokeAllRequest? request)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var count = await _sessions.RevokeAllAsync(userId, request?.ExceptRefreshToken);
        return Ok(new { revoked = count });
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
