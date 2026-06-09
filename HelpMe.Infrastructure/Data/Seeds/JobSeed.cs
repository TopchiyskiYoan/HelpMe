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

        var client1 = await userManager.FindByEmailAsync("client@helpme.bg");
        var client2 = await userManager.FindByEmailAsync("maria.ivanova@helpme.bg");
        var client3 = await userManager.FindByEmailAsync("stefan.nikolov@helpme.bg");
        var client4 = await userManager.FindByEmailAsync("elena.dimitrova@helpme.bg");
        var client5 = await userManager.FindByEmailAsync("plamen.stoyanov@helpme.bg");

        var hm1 = await userManager.FindByEmailAsync("handyman@helpme.bg");
        var hm2 = await userManager.FindByEmailAsync("handyman2@helpme.bg");
        var hm3 = await userManager.FindByEmailAsync("handyman3@helpme.bg");
        var hm4 = await userManager.FindByEmailAsync("nikolay.todorov@helpme.bg");
        var hm5 = await userManager.FindByEmailAsync("borislav.marinov@helpme.bg");

        if (client1 is null || hm1 is null) return;

        var subCategories = await context.ServiceSubCategories.ToListAsync();
        var cities = await context.Cities.ToListAsync();

        ServiceSubCategory? Sub(string name) => subCategories.FirstOrDefault(s => s.Name == name);
        City? Cit(string name) => cities.FirstOrDefault(c => c.Name == name);

        var jobs = new List<Job>();

        // ── Completed jobs (with selected handyman) ──────────────────────────
        if (hm1 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Поправка на ел. инсталация в баня",
                Description = "Смяна на стара инсталация в банята — ключове, контакти и осветление. Работата е приключена успешно.",
                ApproximateBudget = 320,
                Status = JobStatus.Completed,
                ClientId = client1.Id,
                SubCategoryId = Sub("Ел. табла")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-60),
                UpdatedAt = DateTime.UtcNow.AddDays(-55),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm1.Id, ProposedPrice = 300, Note = "Мога да се заема веднага.", SubmittedAt = DateTime.UtcNow.AddDays(-59), Status = JobInterestStatus.Selected }
                }
            });

            jobs.Add(new Job
            {
                Title = "Монтаж на ключове и контакти",
                Description = "Нови ключове и контакти в цял апартамент, 5 стаи. Приключено.",
                ApproximateBudget = 250,
                Status = JobStatus.Completed,
                ClientId = client1.Id,
                SubCategoryId = Sub("Ключове и контакти")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-45),
                UpdatedAt = DateTime.UtcNow.AddDays(-40),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm1.Id, ProposedPrice = 240, Note = "Имам опит с апартаменти.", SubmittedAt = DateTime.UtcNow.AddDays(-44), Status = JobInterestStatus.Selected }
                }
            });
        }

        if (hm2 is not null && client2 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Ремонт на теч в баня",
                Description = "Течащ кран в банята. Проблемът е отстранен.",
                ApproximateBudget = 120,
                Status = JobStatus.Completed,
                ClientId = client2.Id,
                SubCategoryId = Sub("Течове")?.Id ?? subCategories[0].Id,
                CityId = Cit("Пловдив")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-50),
                UpdatedAt = DateTime.UtcNow.AddDays(-45),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm2.Id, ProposedPrice = 110, Note = "Специалист по ВиК.", SubmittedAt = DateTime.UtcNow.AddDays(-49), Status = JobInterestStatus.Selected }
                }
            });

            jobs.Add(new Job
            {
                Title = "Полагане на плочки в баня",
                Description = "Баня 6 кв.м., пълна подова и стенна облицовка. Работата е завършена.",
                ApproximateBudget = 1200,
                Status = JobStatus.Completed,
                ClientId = client2.Id,
                SubCategoryId = Sub("Плочки")?.Id ?? subCategories[0].Id,
                CityId = Cit("Пловдив")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-35),
                UpdatedAt = DateTime.UtcNow.AddDays(-28),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm2.Id, ProposedPrice = 1100, Note = "Работя с качествени материали.", SubmittedAt = DateTime.UtcNow.AddDays(-34), Status = JobInterestStatus.Selected }
                }
            });
        }

        if (hm3 is not null && client3 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Монтаж на паркет в хол",
                Description = "Хол 25 кв.м. — монтаж на ламинат с подложка. Приключено.",
                ApproximateBudget = 800,
                Status = JobStatus.Completed,
                ClientId = client3.Id,
                SubCategoryId = Sub("Паркет")?.Id ?? subCategories[0].Id,
                CityId = Cit("Варна")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm3.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-40),
                UpdatedAt = DateTime.UtcNow.AddDays(-33),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm3.Id, ProposedPrice = 750, Note = "Бърза и качествена работа.", SubmittedAt = DateTime.UtcNow.AddDays(-39), Status = JobInterestStatus.Selected }
                }
            });
        }

        if (hm4 is not null && client4 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Шпакловка и боядисване на апартамент",
                Description = "3-стаен апартамент, 75 кв.м. — цялостна шпакловка и боядисване. Приключено.",
                ApproximateBudget = 2500,
                Status = JobStatus.Completed,
                ClientId = client4.Id,
                SubCategoryId = Sub("Гипсова шпакловка")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm4.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-55),
                UpdatedAt = DateTime.UtcNow.AddDays(-44),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm4.Id, ProposedPrice = 2400, Note = "Работя с екип, бърза доставка.", SubmittedAt = DateTime.UtcNow.AddDays(-54), Status = JobInterestStatus.Selected }
                }
            });
        }

        if (hm5 is not null && client5 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Монтаж на климатик",
                Description = "Монтаж на Mitsubishi 12000 BTU в спалня. Приключено.",
                ApproximateBudget = 200,
                Status = JobStatus.Completed,
                ClientId = client5.Id,
                SubCategoryId = Sub("Климатици")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm5.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-30),
                UpdatedAt = DateTime.UtcNow.AddDays(-28),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm5.Id, ProposedPrice = 180, Note = "Имам инструменти за монтаж.", SubmittedAt = DateTime.UtcNow.AddDays(-29), Status = JobInterestStatus.Selected }
                }
            });
        }

        // ── InProgress jobs ───────────────────────────────────────────────────
        if (hm1 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Смяна на ел. табло",
                Description = "Нуждая се от смяна на старо ел. табло с ново. Апартамент 80 кв.м. В процес.",
                ApproximateBudget = 400,
                Status = JobStatus.InProgress,
                ClientId = client1.Id,
                SubCategoryId = Sub("Ел. табла")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm1.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                UpdatedAt = DateTime.UtcNow.AddDays(-7),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm1.Id, ProposedPrice = 380, Note = "Мога да се заема тази седмица.", SubmittedAt = DateTime.UtcNow.AddDays(-9), Status = JobInterestStatus.Selected }
                }
            });
        }

        if (hm2 is not null && client2 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Полагане на ламинат в детска стая",
                Description = "Детска стая 12 кв.м. с демонтаж на стар паркет. В процес.",
                ApproximateBudget = 350,
                Status = JobStatus.InProgress,
                ClientId = client2.Id,
                SubCategoryId = Sub("Паркет")?.Id ?? subCategories[0].Id,
                CityId = Cit("Пловдив")?.Id ?? cities[0].Id,
                SelectedHandymanId = hm2.Id,
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-5),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm2.Id, ProposedPrice = 320, Note = "Имам наличен материал.", SubmittedAt = DateTime.UtcNow.AddDays(-7), Status = JobInterestStatus.Selected }
                }
            });
        }

        // ── Open jobs (with interests) ────────────────────────────────────────
        if (client3 is not null && hm1 is not null && hm3 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Монтаж на осветление в кухня",
                Description = "Монтаж на 6 броя LED луни и 1 висяща лампа над масата.",
                ApproximateBudget = 150,
                Status = JobStatus.Open,
                ClientId = client3.Id,
                SubCategoryId = Sub("Осветление")?.Id ?? subCategories[0].Id,
                CityId = Cit("Варна")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-4),
                UpdatedAt = DateTime.UtcNow.AddDays(-4),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm1.Id, ProposedPrice = 140, Note = "Мога утре.", SubmittedAt = DateTime.UtcNow.AddDays(-3), Status = JobInterestStatus.Pending },
                    new() { HandymanId = hm3.Id, ProposedPrice = 130, Note = "Имам опит с осветление.", SubmittedAt = DateTime.UtcNow.AddDays(-2), Status = JobInterestStatus.Pending }
                }
            });
        }

        if (client4 is not null && hm4 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Боядисване на фасада",
                Description = "Двуетажна къща, фасадно боядисване след ремонт на мазилка.",
                ApproximateBudget = 1800,
                Status = JobStatus.Open,
                ClientId = client4.Id,
                SubCategoryId = Sub("Фасадно боядисване")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm4.Id, ProposedPrice = 1700, Note = "Работя с специализирана фасадна боя.", SubmittedAt = DateTime.UtcNow.AddDays(-2), Status = JobInterestStatus.Pending }
                }
            });
        }

        if (client5 is not null && hm5 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Сервиз на климатик",
                Description = "Профилактика и дозареждане на климатик Daikin. Да се провери и дренажна тръба.",
                ApproximateBudget = 120,
                Status = JobStatus.Open,
                ClientId = client5.Id,
                SubCategoryId = Sub("Климатици")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2),
                Interests = new List<JobInterest>
                {
                    new() { HandymanId = hm5.Id, ProposedPrice = 100, Note = "Специализиран в Daikin.", SubmittedAt = DateTime.UtcNow.AddDays(-1), Status = JobInterestStatus.Pending }
                }
            });
        }

        // ── Open jobs (no interests yet) ──────────────────────────────────────
        jobs.Add(new Job
        {
            Title = "Течове от кухненска мивка",
            Description = "Тече тръбата под мивката в кухнята. Необходима спешна помощ.",
            ApproximateBudget = 80,
            Status = JobStatus.Open,
            ClientId = client1.Id,
            SubCategoryId = Sub("Течове")?.Id ?? subCategories[0].Id,
            CityId = Cit("София")?.Id ?? cities[0].Id,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        });

        if (client2 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Монтаж на душ кабина",
                Description = "Монтаж на квадратна душ кабина 80x80, включително силикониране.",
                ApproximateBudget = 200,
                Status = JobStatus.Open,
                ClientId = client2.Id,
                SubCategoryId = Sub("Аксесоари баня")?.Id ?? subCategories[0].Id,
                CityId = Cit("Пловдив")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddHours(-6),
                UpdatedAt = DateTime.UtcNow.AddHours(-6)
            });
        }

        if (client3 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Изграждане на преградна стена",
                Description = "Разделяне на голяма стая с гипсокартонена стена, 10 кв.м.",
                ApproximateBudget = 600,
                Status = JobStatus.Open,
                ClientId = client3.Id,
                SubCategoryId = Sub("Преградни стени")?.Id ?? subCategories[0].Id,
                CityId = Cit("Варна")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddHours(-12),
                UpdatedAt = DateTime.UtcNow.AddHours(-12)
            });
        }

        if (client4 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Монтаж на кухня",
                Description = "Сглобяване и монтаж на нова кухня — 10 модула, вграждане на печка и абсорбатор.",
                ApproximateBudget = 500,
                Status = JobStatus.Open,
                ClientId = client4.Id,
                SubCategoryId = Sub("Кухни монтаж")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddHours(-18),
                UpdatedAt = DateTime.UtcNow.AddHours(-18)
            });
        }

        if (client5 is not null)
        {
            jobs.Add(new Job
            {
                Title = "Полагане на хидроизолация в баня",
                Description = "Преди полагане на плочки — пълна хидроизолация на пода и долен пояс на стените, баня 5 кв.м.",
                ApproximateBudget = 300,
                Status = JobStatus.Open,
                ClientId = client5.Id,
                SubCategoryId = Sub("Хидроизолация баня")?.Id ?? subCategories[0].Id,
                CityId = Cit("София")?.Id ?? cities[0].Id,
                CreatedAt = DateTime.UtcNow.AddHours(-3),
                UpdatedAt = DateTime.UtcNow.AddHours(-3)
            });
        }

        // ── Cancelled job ─────────────────────────────────────────────────────
        jobs.Add(new Job
        {
            Title = "Полагане на плочки в тераса",
            Description = "Тераса 8 кв.м., отменено поради промяна в плановете.",
            ApproximateBudget = 700,
            Status = JobStatus.Cancelled,
            ClientId = client1.Id,
            SubCategoryId = Sub("Плочки")?.Id ?? subCategories[0].Id,
            CityId = Cit("София")?.Id ?? cities[0].Id,
            CreatedAt = DateTime.UtcNow.AddDays(-20),
            UpdatedAt = DateTime.UtcNow.AddDays(-18)
        });

        await context.Jobs.AddRangeAsync(jobs);
        await context.SaveChangesAsync();
    }
}
