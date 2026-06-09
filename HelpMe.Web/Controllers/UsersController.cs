using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var user = await _userService.GetUserByIdAsync(UserId);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var success = await _userService.UpdateProfileAsync(UserId, dto);
        if (!success) return NotFound();
        var user = await _userService.GetUserByIdAsync(UserId);
        return Ok(user);
    }

    [HttpPut("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var (success, error) = await _userService.ChangePasswordAsync(UserId, dto);
        if (!success)
            return BadRequest(new { error });
        return NoContent();
    }
}
