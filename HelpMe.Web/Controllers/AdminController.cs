using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Administrator")]
public class AdminController : ControllerBase
{
    private readonly IAdminService _adminService;
    private readonly IReviewService _reviewService;

    public AdminController(IAdminService adminService, IReviewService reviewService)
    {
        _adminService = adminService;
        _reviewService = reviewService;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers([FromQuery] string? search, [FromQuery] int page = 1)
    {
        var result = await _adminService.GetAllUsersAsync(search, page);
        return Ok(result);
    }

    [HttpPost("users/{id}/ban")]
    public async Task<IActionResult> BanUser(string id)
    {
        var success = await _adminService.BanUserAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpPost("users/{id}/unban")]
    public async Task<IActionResult> UnbanUser(string id)
    {
        var success = await _adminService.UnbanUserAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpGet("jobs")]
    public async Task<IActionResult> GetJobs([FromQuery] string? status, [FromQuery] int page = 1)
    {
        var result = await _adminService.GetAllJobsAsync(status, page);
        return Ok(result);
    }

    [HttpGet("reviews")]
    public async Task<IActionResult> GetReviews([FromQuery] int page = 1)
    {
        var result = await _adminService.GetAllReviewsAsync(page);
        return Ok(result);
    }

    [HttpDelete("reviews/{id:int}")]
    public async Task<IActionResult> DeleteReview(int id)
    {
        var success = await _reviewService.DeleteReviewAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
