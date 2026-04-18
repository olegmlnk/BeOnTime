using System.Security.Claims;
using BeOnTime.Application.DTOs.Tasks;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] TaskFilterDto filter)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var tasks = await _taskService.GetAllAsync(userId, filter);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var task = await _taskService.GetByIdAsync(userId, id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var task = await _taskService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var task = await _taskService.UpdateAsync(userId, id, dto);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateTaskStatusDto dto)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var task = await _taskService.UpdateStatusAsync(userId, id, dto.Status);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var deleted = await _taskService.DeleteAsync(userId, id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("overdue")]
    public async Task<IActionResult> GetOverdue()
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var tasks = await _taskService.GetOverdueAsync(userId);
        return Ok(tasks);
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
    {
        if (!TryGetUserId(out var userId)) return Unauthorized();
        var tasks = await _taskService.GetUpcomingAsync(userId, days);
        return Ok(tasks);
    }

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim, out userId);
    }
}
