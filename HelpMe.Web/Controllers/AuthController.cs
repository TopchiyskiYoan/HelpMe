using System.Security.Claims;
using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpMe.Web.Controllers;

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
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);

        if (!result.Succeeded)
        {
            return result.ErrorCode switch
            {
                "EMAIL_EXISTS" => Conflict(new { message = "Email already in use." }),
                "INVALID_ROLE" => BadRequest(new { message = "Invalid role. Registration is only allowed for 'Client' or 'Handyman'." }),
                "INVALID_PHONE" => BadRequest(new { message = "Invalid phone number. Use 08XXXXXXXX or +359XXXXXXXXX." }),
                _ => BadRequest(new { message = "Registration failed." })
            };
        }

        return Ok(result.Data);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _authService.LoginAsync(dto);

        if (!result.Succeeded)
            return Unauthorized(new { message = "Invalid email or password." });

        return Ok(result.Data);
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        // JWT е stateless — клиентът изтрива токена локално
        return Ok(new { message = "Logged out successfully." });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var user = await _authService.GetCurrentUserAsync(userId);
        if (user is null) return NotFound();

        return Ok(user);
    }
}
