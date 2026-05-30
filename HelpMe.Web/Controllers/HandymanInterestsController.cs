using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/handymen/me/interests")]
[Authorize(Roles = "Handyman")]
public class HandymanInterestsController : ControllerBase
{
    private readonly IJobInterestService _jobInterestService;

    public HandymanInterestsController(IJobInterestService jobInterestService)
    {
        _jobInterestService = jobInterestService;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyInterests()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var interests = await _jobInterestService.GetHandymanInterestsAsync(userId);
        return Ok(interests);
    }
}
