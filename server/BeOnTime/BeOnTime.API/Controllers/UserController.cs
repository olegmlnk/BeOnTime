using Microsoft.AspNetCore.Mvc;

namespace BeOnTime.API.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    
    public UserController(ILogger<UserController> logger)
    { 
        _logger = logger;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetUserById(Guid id)
    {
        return Ok("Hello World);");
    }
}