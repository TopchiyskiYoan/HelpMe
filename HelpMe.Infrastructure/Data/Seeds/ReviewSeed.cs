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

        var reviewData = new[]
        {
            (Rating: 5, Comment: "Страхотна работа! Майсторът беше точен, професионален и почисти след себе си. Горещо препоръчвам!"),
            (Rating: 5, Comment: "Отлично качество на изпълнение. Работи бързо и спазва сроковете. Ще се обадя отново!"),
            (Rating: 4, Comment: "Много добра работа, доволен съм от резултата. Малко закъсня, но компенсира с качество."),
            (Rating: 5, Comment: "Перфектна работа! Внимание към детайла, чистота и отговорност. Препоръчвам без резерви."),
            (Rating: 4, Comment: "Добро качество, конкурентна цена. Ще го препоръчам на приятели."),
            (Rating: 3, Comment: "Работата е приемлива, но имаше нужда от корекции. Общо взето доволен."),
            (Rating: 5, Comment: "Най-добрият майстор, с когото съм работил. Бърз, прецизен и коректен в цената."),
            (Rating: 4, Comment: "Много доволен от резултата. Ще се обърна към него при следващ ремонт."),
        };

        var reviews = new List<Review>();
        var idx = 0;

        foreach (var job in completedJobs)
        {
            if (reviews.Any(r => r.JobId == job.Id)) continue;

            var (rating, comment) = reviewData[idx % reviewData.Length];
            reviews.Add(new Review
            {
                JobId = job.Id,
                ClientId = job.ClientId,
                HandymanId = job.SelectedHandymanId!,
                Rating = rating,
                Comment = comment,
                CreatedAt = job.UpdatedAt.AddDays(1)
            });
            idx++;
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
