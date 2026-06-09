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
                Bio = "Лицензиран електротехник с над 10 години опит в жилищни и търговски обекти. Монтаж и ремонт на ел. табла, инсталации, ключове, контакти и осветително оборудване. Работя бързо, чисто и с гаранция за качество.",
                YearsOfExperience = 10,
                IsVerified = true,
                SubCategoryNames = new[] { "Ел. табла", "Ключове и контакти", "Осветление" },
                CityNames = new[] { "София", "Банкя", "Бояна" }
            },
            new
            {
                Email = "handyman2@helpme.bg",
                Bio = "Специалист по облицовки и настилки с 7-годишен опит. Полагане на плочки в бани, кухни и тераси, монтаж на паркет и ламинат, хидроизолация. Прецизна работа с внимание към детайла.",
                YearsOfExperience = 7,
                IsVerified = true,
                SubCategoryNames = new[] { "Плочки", "Паркет", "Хидроизолация баня" },
                CityNames = new[] { "Пловдив", "Асеновград", "Марица" }
            },
            new
            {
                Email = "handyman3@helpme.bg",
                Bio = "ВиК майстор с 12 години стаж. Изграждане и ремонт на водопроводни и канализационни системи, монтаж на санитария, отстраняване на течове. Работя 7 дни в седмицата, включително при аварии.",
                YearsOfExperience = 12,
                IsVerified = true,
                SubCategoryNames = new[] { "Водопровод и канализация", "Монтаж санитария", "Течове" },
                CityNames = new[] { "Варна", "Белослав", "Провадия" }
            },
            new
            {
                Email = "nikolay.todorov@helpme.bg",
                Bio = "Майстор-строител с 15 години опит в ремонти и довършителни работи. Изграждане на преградни стени, шпакловка, боядисване, поставяне на окачени тавани. Работя с екип за по-бързо изпълнение.",
                YearsOfExperience = 15,
                IsVerified = true,
                SubCategoryNames = new[] { "Гипсова шпакловка", "Фино боядисване", "Преградни стени", "Окачени тавани" },
                CityNames = new[] { "София", "Пловдив" }
            },
            new
            {
                Email = "borislav.marinov@helpme.bg",
                Bio = "Климатичен техник с 8 години опит. Монтаж, демонтаж и сервиз на климатични системи от всички марки. Бърза реакция и конкурентни цени.",
                YearsOfExperience = 8,
                IsVerified = true,
                SubCategoryNames = new[] { "Климатици", "Вентилация", "Радиатори" },
                CityNames = new[] { "София", "Самоков" }
            },
            new
            {
                Email = "atanas.popov@helpme.bg",
                Bio = "Дограмаджия и стъклар с 6 години опит. Монтаж на PVC и алуминиева дограма, ремонт на прозорци и врати, поставяне на стъкло. Работя прецизно и в договорените срокове.",
                YearsOfExperience = 6,
                IsVerified = false,
                SubCategoryNames = new[] { "Демонтаж дограма", "Врати монтаж" },
                CityNames = new[] { "Варна", "Бургас" }
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
                IsVerified = profile.IsVerified,
                SubCategories = subCategoryIds.Select(id => new HandymanSubCategory { UserId = user.Id, SubCategoryId = id }).ToList(),
                Cities = cityIds.Select(id => new HandymanCity { UserId = user.Id, CityId = id }).ToList()
            };

            await context.HandymanProfiles.AddAsync(handymanProfile);
        }

        await context.SaveChangesAsync();
    }
}
