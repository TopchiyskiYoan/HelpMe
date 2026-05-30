using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HelpMe.Web.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController : ControllerBase
{
    private readonly IJobService _jobService;
    private readonly IJobInterestService _jobInterestService;

    public JobsController(IJobService jobService, IJobInterestService jobInterestService)
    {
        _jobService = jobService;
        _jobInterestService = jobInterestService;
    }

    [HttpPost]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Create([FromBody] CreateJobDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var job = await _jobService.CreateAsync(userId, dto);
        return CreatedAtAction(nameof(GetById), new { id = job.Id }, job);
    }

    [HttpGet("my")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetMyJobs()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var jobs = await _jobService.GetClientJobsAsync(userId);
        return Ok(jobs);
    }

    [HttpGet("feed")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> GetFeed()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var jobs = await _jobService.GetOpenJobsForHandymanAsync(userId);
        return Ok(jobs);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var job = await _jobService.GetByIdAsync(id);
        if (job is null) return NotFound();

        return Ok(job);
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _jobService.CancelAsync(id, userId);
        if (!success) return BadRequest(new { message = "Cannot cancel this job. It may not exist, belong to you, or be in a cancellable state." });

        return NoContent();
    }

    [HttpPost("{jobId:int}/select/{interestId:int}")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> SelectHandyman(int jobId, int interestId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _jobService.SelectHandymanAsync(jobId, interestId, userId);
        if (!success) return BadRequest(new { message = "Cannot select handyman. Job must be Open and you must be the client." });

        return NoContent();
    }

    [HttpPost("{jobId:int}/confirm")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> Confirm(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _jobService.ConfirmJobAsync(jobId, userId);
        if (!success) return BadRequest(new { message = "Cannot confirm this job. It must be awaiting your confirmation." });

        return NoContent();
    }

    [HttpPost("{jobId:int}/decline")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> Decline(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _jobService.DeclineJobAsync(jobId, userId);
        if (!success) return BadRequest(new { message = "Cannot decline this job. It must be awaiting your confirmation." });

        return NoContent();
    }

    [HttpPost("{jobId:int}/complete")]
    public async Task<IActionResult> Complete(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var success = await _jobService.CompleteJobAsync(jobId, userId);
        if (!success) return BadRequest(new { message = "Cannot complete this job. It must be InProgress and you must be the client or selected handyman." });

        return NoContent();
    }

    // --- Job Interests (nested under jobs) ---

    [HttpPost("{jobId:int}/interests")]
    [Authorize(Roles = "Handyman")]
    public async Task<IActionResult> SubmitInterest(int jobId, [FromBody] SubmitInterestDto dto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var interest = await _jobInterestService.SubmitInterestAsync(userId, jobId, dto);
        if (interest is null) return BadRequest(new { message = "Cannot submit interest. Job must be Open and you must not have already submitted." });

        return CreatedAtAction(nameof(GetInterestsForJob), new { jobId }, interest);
    }

    [HttpGet("{jobId:int}/interests")]
    [Authorize(Roles = "Client")]
    public async Task<IActionResult> GetInterestsForJob(int jobId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var job = await _jobService.GetByIdAsync(jobId);
        if (job is null) return NotFound();
        if (job.ClientId != userId) return Forbid();

        var interests = await _jobInterestService.GetInterestsForJobAsync(jobId);
        return Ok(interests);
    }
}
