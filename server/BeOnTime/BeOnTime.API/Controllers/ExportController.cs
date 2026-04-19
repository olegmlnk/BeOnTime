using System.Security.Claims;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/export")]
[Authorize]
public class ExportController : ControllerBase
{
    private readonly IExportService _export;

    public ExportController(IExportService export)
    {
        _export = export;
    }

    [HttpPost]
    public async Task<IActionResult> Enqueue()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var job = await _export.EnqueueAsync(userId);
        return AcceptedAtAction(nameof(GetStatus), new { id = job.Id }, job);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetStatus(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var job = await _export.GetStatusAsync(userId, id);
        return job is null ? NotFound() : Ok(job);
    }

    [HttpGet("{id:guid}/download")]
    public async Task<IActionResult> Download(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var file = await _export.GetFileAsync(userId, id);
        if (file is null) return NotFound();

        var (content, fileName, contentType) = file.Value;
        return File(content, contentType, fileName);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
