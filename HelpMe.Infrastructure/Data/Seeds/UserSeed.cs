using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class UserSeed
{
    private const string DefaultPassword = "Test1234!";

    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        var users = new[]
        {
            (new ApplicationUser
            {
                UserName = "admin@helpme.bg",
                Email = "admin@helpme.bg",
                FirstName = "Администратор",
                LastName = "HelpMe",
                PhoneNumber = "0888000001",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-120)
            }, "Administrator"),

            (new ApplicationUser
            {
                UserName = "client@helpme.bg",
                Email = "client@helpme.bg",
                FirstName = "Георги",
                LastName = "Петров",
                PhoneNumber = "0888100001",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-90)
            }, "Client"),

            (new ApplicationUser
            {
                UserName = "maria.ivanova@helpme.bg",
                Email = "maria.ivanova@helpme.bg",
                FirstName = "Мария",
                LastName = "Иванова",
                PhoneNumber = "0888100002",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-75)
            }, "Client"),

            (new ApplicationUser
            {
                UserName = "stefan.nikolov@helpme.bg",
                Email = "stefan.nikolov@helpme.bg",
                FirstName = "Стефан",
                LastName = "Николов",
                PhoneNumber = "0888100003",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-60)
            }, "Client"),

            (new ApplicationUser
            {
                UserName = "elena.dimitrova@helpme.bg",
                Email = "elena.dimitrova@helpme.bg",
                FirstName = "Елена",
                LastName = "Димитрова",
                PhoneNumber = "0888100004",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-45)
            }, "Client"),

            (new ApplicationUser
            {
                UserName = "plamen.stoyanov@helpme.bg",
                Email = "plamen.stoyanov@helpme.bg",
                FirstName = "Пламен",
                LastName = "Стоянов",
                PhoneNumber = "0888100005",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-30)
            }, "Client"),

            (new ApplicationUser
            {
                UserName = "handyman@helpme.bg",
                Email = "handyman@helpme.bg",
                FirstName = "Димитър",
                LastName = "Колев",
                PhoneNumber = "0888200001",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-100)
            }, "Handyman"),

            (new ApplicationUser
            {
                UserName = "handyman2@helpme.bg",
                Email = "handyman2@helpme.bg",
                FirstName = "Иван",
                LastName = "Георгиев",
                PhoneNumber = "0888200002",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-95)
            }, "Handyman"),

            (new ApplicationUser
            {
                UserName = "handyman3@helpme.bg",
                Email = "handyman3@helpme.bg",
                FirstName = "Стоян",
                LastName = "Христов",
                PhoneNumber = "0888200003",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-85)
            }, "Handyman"),

            (new ApplicationUser
            {
                UserName = "nikolay.todorov@helpme.bg",
                Email = "nikolay.todorov@helpme.bg",
                FirstName = "Николай",
                LastName = "Тодоров",
                PhoneNumber = "0888200004",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-70)
            }, "Handyman"),

            (new ApplicationUser
            {
                UserName = "borislav.marinov@helpme.bg",
                Email = "borislav.marinov@helpme.bg",
                FirstName = "Борислав",
                LastName = "Маринов",
                PhoneNumber = "0888200005",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-55)
            }, "Handyman"),

            (new ApplicationUser
            {
                UserName = "atanas.popov@helpme.bg",
                Email = "atanas.popov@helpme.bg",
                FirstName = "Атанас",
                LastName = "Попов",
                PhoneNumber = "0888200006",
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow.AddDays(-40)
            }, "Handyman"),
        };

        foreach (var (user, role) in users)
        {
            if (await userManager.FindByEmailAsync(user.Email!) is not null)
                continue;

            await userManager.CreateAsync(user, DefaultPassword);
            await userManager.AddToRoleAsync(user, role);
        }
    }
}
