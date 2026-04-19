using System.Security.Claims;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/archive")]
[Authorize]
public class ArchiveController : ControllerBase
{
    private readonly IArchiveService _archive;

    public ArchiveController(IArchiveService archive)
    {
        _archive = archive;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var items = await _archive.GetAllAsync(userId);
        return Ok(items);
    }

    [HttpPost("{type}/{id:guid}/restore")]
    public async Task<IActionResult> Restore(string type, Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var ok = await _archive.RestoreAsync(userId, type, id);
        return ok ? NoContent() : NotFound();
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
