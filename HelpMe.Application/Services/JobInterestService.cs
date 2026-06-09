using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class JobInterestService : IJobInterestService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;

    public JobInterestService(IApplicationDbContext context, INotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task<JobInterestDto?> SubmitInterestAsync(string handymanUserId, int jobId, SubmitInterestDto dto)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);
        if (job is null || job.Status != JobStatus.Open) return null;

        var alreadySubmitted = await _context.JobInterests
            .AnyAsync(i => i.JobId == jobId && i.HandymanId == handymanUserId);
        if (alreadySubmitted) return null;

        var interest = new JobInterest
        {
            JobId = jobId,
            HandymanId = handymanUserId,
            ProposedPrice = dto.ProposedPrice,
            Note = dto.Note?.Trim(),
            SubmittedAt = DateTime.UtcNow,
            Status = JobInterestStatus.Pending
        };

        await _context.JobInterests.AddAsync(interest);
        await _context.SaveChangesAsync();

        var saved = await _context.JobInterests
            .Include(i => i.Handyman)
                .ThenInclude(h => h.User)
            .FirstOrDefaultAsync(i => i.Id == interest.Id);

        if (saved?.Handyman?.User is not null)
        {
            var handymanName = $"{saved.Handyman.User.FirstName} {saved.Handyman.User.LastName}";
            await _notifications.CreateAsync(
                job.ClientId,
                NotificationType.JobInterestReceived,
                "Нов интерес към поръчката ви",
                $"{handymanName} изрази интерес към \"{job.Title}\".");
        }

        return saved is null ? null : ToDto(saved);
    }

    public async Task<List<JobInterestDto>> GetInterestsForJobAsync(int jobId)
    {
        var interests = await _context.JobInterests
            .Where(i => i.JobId == jobId)
            .Include(i => i.Handyman)
                .ThenInclude(h => h.User)
            .OrderBy(i => i.SubmittedAt)
            .ToListAsync();

        return interests.Select(ToDto).ToList();
    }

    public async Task<List<JobInterestDto>> GetHandymanInterestsAsync(string handymanUserId)
    {
        var interests = await _context.JobInterests
            .Where(i => i.HandymanId == handymanUserId)
            .Include(i => i.Handyman)
                .ThenInclude(h => h.User)
            .OrderByDescending(i => i.SubmittedAt)
            .ToListAsync();

        return interests.Select(ToDto).ToList();
    }

    private static JobInterestDto ToDto(JobInterest i) => new()
    {
        Id = i.Id,
        JobId = i.JobId,
        HandymanId = i.HandymanId,
        HandymanName = i.Handyman?.User is not null
            ? $"{i.Handyman.User.FirstName} {i.Handyman.User.LastName}"
            : string.Empty,
        ProposedPrice = i.ProposedPrice,
        Note = i.Note,
        SubmittedAt = i.SubmittedAt,
        Status = i.Status.ToString()
    };
}
