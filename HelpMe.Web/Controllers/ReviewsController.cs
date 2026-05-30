using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/reviews")]
[Authorize]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var review = await _reviewService.CreateReviewAsync(userId, dto);
        if (review is null)
            return BadRequest(new { message = "Cannot create review. The job must be Completed, you must be the client, and no review must already exist." });

        return CreatedAtAction(nameof(GetHandymanReviews), new { handymanId = review.HandymanId }, review);
    }

    [HttpGet("handyman/{handymanId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHandymanReviews(string handymanId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var reviews = await _reviewService.GetHandymanReviewsAsync(handymanId, page, pageSize);
        return Ok(reviews);
    }

    [HttpPost("{id:int}/respond")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> Respond(int id, [FromBody] RespondToReviewDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _reviewService.RespondToReviewAsync(userId, id, dto.Content);
        if (!success)
            return BadRequest(new { message = "Cannot respond. You must be the reviewed handyman and this review must not already have a response." });

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _reviewService.DeleteReviewAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
