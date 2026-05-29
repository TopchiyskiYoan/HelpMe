using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/handymen")]
public class HandymanProfilesController : ControllerBase
{
    private readonly IHandymanProfileService _handymanProfileService;

    public HandymanProfilesController(IHandymanProfileService handymanProfileService)
    {
        _handymanProfileService = handymanProfileService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var profiles = await _handymanProfileService.GetAllAsync();
        return Ok(profiles);
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetByUserId(string userId)
    {
        var profile = await _handymanProfileService.GetPublicProfileAsync(userId);
        if (profile is null) return NotFound();

        return Ok(profile);
    }

    [HttpGet("me")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var profile = await _handymanProfileService.GetByUserIdAsync(userId);
        if (profile is null) return NotFound();

        return Ok(profile);
    }

    [HttpPost("me")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> Create([FromBody] CreateHandymanProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var created = await _handymanProfileService.CreateAsync(userId, dto);
        if (created is null) return Conflict(new { message = "Profile already exists." });

        return CreatedAtAction(nameof(GetByUserId), new { userId = created.UserId }, created);
    }

    [HttpPut("me")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> Update([FromBody] UpdateHandymanProfileDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _handymanProfileService.UpdateAsync(userId, dto);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{userId}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Deactivate(string userId)
    {
        var success = await _handymanProfileService.DeactivateAsync(userId);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpGet("/api/admin/handymen/pending")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetPending()
    {
        var profiles = await _handymanProfileService.GetPendingVerificationAsync();
        return Ok(profiles);
    }

    [HttpPost("/api/admin/handymen/{userId}/verify")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Verify(string userId, [FromBody] VerifyHandymanDto dto)
    {
        var success = await _handymanProfileService.VerifyAsync(userId, dto.Approved);
        if (!success) return NotFound();

        return NoContent();
    }
}
