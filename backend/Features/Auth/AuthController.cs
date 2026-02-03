using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Features.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _service;

    public AuthController(AuthService service)
    {
        _service = service;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    => Ok(await _service.LoginAsync(req));


    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest req)
        => Ok(await _service.RefreshAsync(req));

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        => Ok(await _service.RegisterAsync(req));

}

