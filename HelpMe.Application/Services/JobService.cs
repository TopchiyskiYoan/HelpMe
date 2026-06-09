using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class JobService : IJobService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notifications;

    public JobService(IApplicationDbContext context, INotificationService notifications)
    {
        _context = context;
        _notifications = notifications;
    }

    public async Task<JobDto> CreateAsync(string clientId, CreateJobDto dto)
    {
        var job = new Job
        {
            Title = dto.Title.Trim(),
            Description = dto.Description.Trim(),
            ApproximateBudget = dto.ApproximateBudget,
            ClientId = clientId,
            SubCategoryId = dto.SubCategoryId,
            CityId = dto.CityId,
            Status = JobStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _context.Jobs.AddAsync(job);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(job.Id) ?? ToDto(job);
    }

    public async Task<JobDto?> GetByIdAsync(int id)
    {
        var job = await _context.Jobs
            .Include(j => j.Client)
            .Include(j => j.SubCategory)
            .Include(j => j.City)
            .Include(j => j.SelectedHandyman)
                .ThenInclude(h => h!.User)
            .Include(j => j.Interests)
                .ThenInclude(i => i.Handyman)
                    .ThenInclude(h => h.User)
            .FirstOrDefaultAsync(j => j.Id == id);

        return job is null ? null : ToDto(job);
    }

    public async Task<List<JobDto>> GetClientJobsAsync(string clientId)
    {
        var jobs = await _context.Jobs
            .Where(j => j.ClientId == clientId)
            .Include(j => j.Client)
            .Include(j => j.SubCategory)
            .Include(j => j.City)
            .Include(j => j.SelectedHandyman)
                .ThenInclude(h => h!.User)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return jobs.Select(ToDto).ToList();
    }

    public async Task<List<JobDto>> GetOpenJobsForHandymanAsync(string handymanUserId)
    {
        var handyman = await _context.HandymanProfiles
            .Include(h => h.SubCategories)
            .Include(h => h.Cities)
            .FirstOrDefaultAsync(h => h.UserId == handymanUserId && h.IsActive && h.IsVerified);

        if (handyman is null) return new List<JobDto>();

        var subCategoryIds = handyman.SubCategories.Select(s => s.SubCategoryId).ToHashSet();
        var cityIds = handyman.Cities.Select(c => c.CityId).ToHashSet();

        var jobs = await _context.Jobs
            .Where(j => j.Status == JobStatus.Open
                && subCategoryIds.Contains(j.SubCategoryId)
                && cityIds.Contains(j.CityId))
            .Include(j => j.Client)
            .Include(j => j.SubCategory)
            .Include(j => j.City)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync();

        return jobs.Select(ToDto).ToList();
    }

    public async Task<bool> CancelAsync(int id, string requesterId)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);

        if (job is null) return false;
        if (job.ClientId != requesterId) return false;
        if (job.Status != JobStatus.Open && job.Status != JobStatus.AwaitingConfirmation) return false;

        job.Status = JobStatus.Cancelled;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SelectHandymanAsync(int jobId, int interestId, string clientId)
    {
        var job = await _context.Jobs
            .Include(j => j.Interests)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;
        if (job.ClientId != clientId) return false;
        if (job.Status != JobStatus.Open) return false;

        var selectedInterest = job.Interests.FirstOrDefault(i => i.Id == interestId);
        if (selectedInterest is null) return false;

        job.SelectedHandymanId = selectedInterest.HandymanId;
        job.Status = JobStatus.AwaitingConfirmation;
        job.UpdatedAt = DateTime.UtcNow;

        foreach (var interest in job.Interests)
        {
            interest.Status = interest.Id == interestId
                ? JobInterestStatus.Selected
                : JobInterestStatus.Rejected;
        }

        await _context.SaveChangesAsync();

        await _notifications.CreateAsync(
            selectedInterest.HandymanId,
            NotificationType.HandymanSelected,
            "Избрани сте за поръчка",
            $"Клиентът ви избра за поръчка \"{job.Title}\". Моля потвърдете или откажете.");

        foreach (var rejected in job.Interests.Where(i => i.Id != interestId))
        {
            await _notifications.CreateAsync(
                rejected.HandymanId,
                NotificationType.HandymanRejected,
                "Не бяхте избрани",
                $"За поръчка \"{job.Title}\" беше избран друг майстор.");
        }

        return true;
    }

    public async Task<bool> ConfirmJobAsync(int jobId, string handymanUserId)
    {
        var job = await _context.Jobs
            .Include(j => j.Client)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;
        if (job.Status != JobStatus.AwaitingConfirmation) return false;
        if (job.SelectedHandymanId != handymanUserId) return false;

        job.Status = JobStatus.InProgress;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _notifications.CreateAsync(
            job.ClientId,
            NotificationType.JobConfirmed,
            "Поръчката ви е потвърдена",
            $"Майсторът потвърди поръчка \"{job.Title}\". Работата е в процес.");

        return true;
    }

    public async Task<bool> DeclineJobAsync(int jobId, string handymanUserId)
    {
        var job = await _context.Jobs
            .Include(j => j.Interests)
            .FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;
        if (job.Status != JobStatus.AwaitingConfirmation) return false;
        if (job.SelectedHandymanId != handymanUserId) return false;

        job.Status = JobStatus.Open;
        job.SelectedHandymanId = null;
        job.UpdatedAt = DateTime.UtcNow;

        foreach (var interest in job.Interests)
        {
            interest.Status = JobInterestStatus.Pending;
        }

        await _context.SaveChangesAsync();

        await _notifications.CreateAsync(
            job.ClientId,
            NotificationType.JobDeclined,
            "Майсторът отказа поръчката",
            $"Избраният майстор отказа поръчка \"{job.Title}\". Поръчката е отворена отново.");

        return true;
    }

    public async Task<bool> CompleteJobAsync(int jobId, string requesterId)
    {
        var job = await _context.Jobs.FirstOrDefaultAsync(j => j.Id == jobId);

        if (job is null) return false;
        if (job.Status != JobStatus.InProgress) return false;
        if (job.ClientId != requesterId && job.SelectedHandymanId != requesterId) return false;

        job.Status = JobStatus.Completed;
        job.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _notifications.CreateAsync(
            job.ClientId,
            NotificationType.JobCompleted,
            "Поръчката е завършена",
            $"Поръчка \"{job.Title}\" беше маркирана като завършена.");

        if (job.SelectedHandymanId is not null)
        {
            await _notifications.CreateAsync(
                job.SelectedHandymanId,
                NotificationType.JobCompleted,
                "Поръчката е завършена",
                $"Поръчка \"{job.Title}\" беше маркирана като завършена.");
        }

        return true;
    }

    private static JobDto ToDto(Job j) => new()
    {
        Id = j.Id,
        Title = j.Title,
        Description = j.Description,
        ApproximateBudget = j.ApproximateBudget,
        Status = j.Status.ToString(),
        ClientId = j.ClientId,
        ClientName = j.Client is not null ? $"{j.Client.FirstName} {j.Client.LastName}" : string.Empty,
        SubCategoryId = j.SubCategoryId,
        SubCategoryName = j.SubCategory?.Name ?? string.Empty,
        CityId = j.CityId,
        CityName = j.City?.Name ?? string.Empty,
        SelectedHandymanId = j.SelectedHandymanId,
        SelectedHandymanName = j.SelectedHandyman?.User is not null
            ? $"{j.SelectedHandyman.User.FirstName} {j.SelectedHandyman.User.LastName}"
            : null,
        CreatedAt = j.CreatedAt,
        UpdatedAt = j.UpdatedAt
    };
}
