using BeOnTime.Application.DTOs;
using BeOnTime.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var result = await _authService.RegisterAsync(request);
        if (!result.Success)
            return BadRequest(new { error = result.Error });

        return Ok(result.Token);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var result = await _authService.LoginASync(request);
        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Token);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
    {
        var result = await _authService.RefreshTokenAsync(request);
        if (!result.Success)
            return Unauthorized(new { error = result.Error });

        return Ok(result.Token);
    }
}
