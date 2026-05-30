using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class ReviewSeed
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        if (await context.Reviews.AnyAsync())
            return;

        var completedJobs = await context.Jobs
            .Where(j => j.Status == JobStatus.Completed && j.SelectedHandymanId != null)
            .ToListAsync();

        if (completedJobs.Count == 0)
            return;

        var reviews = new List<Review>();

        foreach (var job in completedJobs)
        {
            var alreadyReviewed = reviews.Any(r => r.JobId == job.Id);
            if (alreadyReviewed) continue;

            reviews.Add(new Review
            {
                JobId = job.Id,
                ClientId = job.ClientId,
                HandymanId = job.SelectedHandymanId!,
                Rating = 5,
                Comment = "Страхотна работа! Майсторът беше точен, професионален и почисти след себе си. Горещо препоръчвам!",
                CreatedAt = job.UpdatedAt.AddDays(1)
            });

            if (reviews.Count >= 2) break;
        }

        if (reviews.Count == 0)
            return;

        await context.Reviews.AddRangeAsync(reviews);
        await context.SaveChangesAsync();

        foreach (var reviewGroup in reviews.GroupBy(r => r.HandymanId))
        {
            var handyman = await context.HandymanProfiles
                .FirstOrDefaultAsync(h => h.UserId == reviewGroup.Key);

            if (handyman is null) continue;

            var allReviews = await context.Reviews
                .Where(r => r.HandymanId == reviewGroup.Key)
                .ToListAsync();

            handyman.ReviewCount = allReviews.Count;
            handyman.AverageRating = allReviews.Count > 0
                ? Math.Round(allReviews.Average(r => r.Rating), 1)
                : 0;
        }

        await context.SaveChangesAsync();
    }
}
