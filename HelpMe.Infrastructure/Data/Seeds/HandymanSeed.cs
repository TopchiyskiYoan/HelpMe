using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class HandymanSeed
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        IApplicationDbContext context)
    {
        if (await context.HandymanProfiles.AnyAsync())
            return;

        var profiles = new[]
        {
            new
            {
                Email = "handyman@helpme.bg",
                Bio = "Електротехник с дългогодишен опит. Монтаж на ел. табла, ключове, контакти и осветление.",
                YearsOfExperience = 10,
                SubCategoryNames = new[] { "Ел. табла", "Ключове и контакти", "Осветление" },
                CityNames = new[] { "София", "Банкя" }
            },
            new
            {
                Email = "handyman2@helpme.bg",
                Bio = "Специалист по облицовки и настилки. Полагане на плочки, паркет и хидроизолация.",
                YearsOfExperience = 7,
                SubCategoryNames = new[] { "Плочки", "Паркет", "Хидроизолация баня" },
                CityNames = new[] { "Пловдив", "Асеновград" }
            },
            new
            {
                Email = "handyman3@helpme.bg",
                Bio = "ВиК майстор. Изграждане на водопроводи, монтаж на санитария и отстраняване на течове.",
                YearsOfExperience = 12,
                SubCategoryNames = new[] { "Водопровод и канализация", "Монтаж санитария", "Течове" },
                CityNames = new[] { "Варна", "Белослав" }
            }
        };

        foreach (var profile in profiles)
        {
            var user = await userManager.FindByEmailAsync(profile.Email);
            if (user is null) continue;

            var subCategoryIds = await context.ServiceSubCategories
                .Where(s => profile.SubCategoryNames.Contains(s.Name))
                .Select(s => s.Id)
                .ToListAsync();

            var cityIds = await context.Cities
                .Where(c => profile.CityNames.Contains(c.Name))
                .Select(c => c.Id)
                .ToListAsync();

            var handymanProfile = new HandymanProfile
            {
                UserId = user.Id,
                Bio = profile.Bio,
                YearsOfExperience = profile.YearsOfExperience,
                SubCategories = subCategoryIds.Select(id => new HandymanSubCategory { SubCategoryId = id }).ToList(),
                Cities = cityIds.Select(id => new HandymanCity { CityId = id }).ToList()
            };

            await context.HandymanProfiles.AddAsync(handymanProfile);
        }

        await context.SaveChangesAsync();
    }
}
