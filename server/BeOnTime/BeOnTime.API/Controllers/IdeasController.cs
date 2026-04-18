using System.Security.Claims;
using BeOnTime.Application.DTOs.Ideas;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/ideas")]
[Authorize]
public class IdeasController : ControllerBase
{
    private readonly IIdeaService _ideaService;

    public IdeasController(IIdeaService ideaService)
    {
        _ideaService = ideaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var ideas = await _ideaService.GetAllAsync(userId);
        return Ok(ideas);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var idea = await _ideaService.GetByIdAsync(userId, id);
        return idea is null ? NotFound() : Ok(idea);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateIdeaDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var idea = await _ideaService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = idea.Id }, idea);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIdeaDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var idea = await _ideaService.UpdateAsync(userId, id, dto);
        return idea is null ? NotFound() : Ok(idea);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var deleted = await _ideaService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/convert")]
    public async Task<IActionResult> ConvertToTask(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();

        var result = await _ideaService.ConvertToTaskAsync(userId, id);
        if (result is null) return NotFound();
        if (!result.Success) return BadRequest(new { error = result.Error });

        return Ok(result.Task);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
