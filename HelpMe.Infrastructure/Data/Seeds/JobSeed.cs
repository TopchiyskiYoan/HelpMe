using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class JobSeed
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        if (await context.Jobs.AnyAsync())
            return;

        var client = await userManager.FindByEmailAsync("client@helpme.bg");
        if (client is null) return;

        var handyman1 = await userManager.FindByEmailAsync("handyman@helpme.bg");
        var handyman2 = await userManager.FindByEmailAsync("handyman2@helpme.bg");

        var subCategories = await context.ServiceSubCategories.ToListAsync();
        var cities = await context.Cities.ToListAsync();

        var elektroSub = subCategories.FirstOrDefault(s => s.Name == "Ел. табла");
        var vikSub = subCategories.FirstOrDefault(s => s.Name == "Течове");
        var plochiSub = subCategories.FirstOrDefault(s => s.Name == "Плочки");

        var sofia = cities.FirstOrDefault(c => c.Name == "София");
        var plovdiv = cities.FirstOrDefault(c => c.Name == "Пловдив");
        var varna = cities.FirstOrDefault(c => c.Name == "Варна");

        if (elektroSub is null || vikSub is null || sofia is null || plovdiv is null) return;

        var jobs = new List<Job>
        {
            new Job
            {
                Title = "Смяна на ел. табло",
                Description = "Нуждая се от смяна на старо ел. табло с ново. Апартамент 80 кв.м.",
                ApproximateBudget = 400,
                Status = JobStatus.Open,
                ClientId = client.Id,
                SubCategoryId = elektroSub.Id,
                CityId = sofia.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-5)
            },
            new Job
            {
                Title = "Течове от кухненска мивка",
                Description = "Тече тръбата под мивката в кухнята. Необходима спешна помощ.",
                ApproximateBudget = 80,
                Status = JobStatus.Open,
                ClientId = client.Id,
                SubCategoryId = vikSub.Id,
                CityId = sofia.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new Job
            {
                Title = "Полагане на плочки в баня",
                Description = "Баня 6 кв.м., нужна подова и стенна облицовка.",
                ApproximateBudget = 1200,
                Status = JobStatus.Cancelled,
                ClientId = client.Id,
                SubCategoryId = plochiSub?.Id ?? elektroSub.Id,
                CityId = plovdiv.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-8)
            }
        };

        if (handyman1 is not null && varna is not null)
        {
            var inProgressJob = new Job
            {
                Title = "Монтаж на ключове и контакти",
                Description = "Нови ключове и контакти в цял апартамент, 5 стаи.",
                ApproximateBudget = 250,
                Status = JobStatus.InProgress,
                ClientId = client.Id,
                SubCategoryId = elektroSub.Id,
                CityId = varna?.Id ?? sofia.Id,
                SelectedHandymanId = handyman1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                Interests = new List<JobInterest>
                {
                    new JobInterest
                    {
                        HandymanId = handyman1.Id,
                        ProposedPrice = 230,
                        Note = "Мога да се заема веднага.",
                        SubmittedAt = DateTime.UtcNow.AddDays(-6),
                        Status = JobInterestStatus.Selected
                    }
                }
            };
            jobs.Add(inProgressJob);

            var completedJob = new Job
            {
                Title = "Поправка на ел. инсталация",
                Description = "Смяна на стара инсталация в баня. Работата е приключена.",
                ApproximateBudget = 320,
                Status = JobStatus.Completed,
                ClientId = client.Id,
                SubCategoryId = elektroSub.Id,
                CityId = sofia.Id,
                SelectedHandymanId = handyman1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-20),
                UpdatedAt = DateTime.UtcNow.AddDays(-15)
            };
            jobs.Add(completedJob);
        }

        if (handyman2 is not null)
        {
            var completedJob2 = new Job
            {
                Title = "Ремонт на теч в баня",
                Description = "Течащ кран в банята. Проблемът е отстранен.",
                ApproximateBudget = 120,
                Status = JobStatus.Completed,
                ClientId = client.Id,
                SubCategoryId = vikSub.Id,
                CityId = sofia.Id,
                SelectedHandymanId = handyman2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-25)
            };
            jobs.Add(completedJob2);
        }

        await context.Jobs.AddRangeAsync(jobs);
        await context.SaveChangesAsync();
    }
}
