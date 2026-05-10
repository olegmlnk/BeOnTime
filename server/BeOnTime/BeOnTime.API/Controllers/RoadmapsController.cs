using System.Security.Claims;
using BeOnTime.Application.DTOs.Roadmaps;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/roadmaps")]
[Authorize]
public class RoadmapsController : ControllerBase
{
    private readonly IRoadmapService _roadmapService;

    public RoadmapsController(IRoadmapService roadmapService)
    {
        _roadmapService = roadmapService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var roadmaps = await _roadmapService.GetAllAsync(userId);
        return Ok(roadmaps);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var roadmap = await _roadmapService.GetByIdAsync(userId, id);
        return roadmap is null ? NotFound() : Ok(roadmap);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoadmapDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _roadmapService.CreateAsync(userId, dto);
        if (!result.Success) return BadRequest(new { error = result.Error });

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoadmapDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _roadmapService.UpdateAsync(userId, id, dto);
        if (result is null) return NotFound();
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var deleted = await _roadmapService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPut("{id:guid}/reorder")]
    public async Task<IActionResult> Reorder(Guid id, [FromBody] ReorderRoadmapDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _roadmapService.ReorderAsync(userId, id, dto);
        if (result is null) return NotFound();
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(result.Value);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
