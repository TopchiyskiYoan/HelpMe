using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class UserSeed
{
    private const string DefaultPassword = "Test1234!";

    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateIfNotExists(userManager, new ApplicationUser
        {
            UserName = "admin@helpme.bg",
            Email = "admin@helpme.bg",
            FirstName = "Admin",
            LastName = "HelpMe",
            PhoneNumber = "0888000001",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Administrator");

        await CreateIfNotExists(userManager, new ApplicationUser
        {
            UserName = "client@helpme.bg",
            Email = "client@helpme.bg",
            FirstName = "Georgi",
            LastName = "Petrov",
            PhoneNumber = "0888000002",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Client");

        await CreateIfNotExists(userManager, new ApplicationUser
        {
            UserName = "handyman@helpme.bg",
            Email = "handyman@helpme.bg",
            FirstName = "Dimitar",
            LastName = "Kolev",
            PhoneNumber = "0888000003",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Handyman");

        await CreateIfNotExists(userManager, new ApplicationUser
        {
            UserName = "handyman2@helpme.bg",
            Email = "handyman2@helpme.bg",
            FirstName = "Ivan",
            LastName = "Georgiev",
            PhoneNumber = "0888000004",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Handyman");

        await CreateIfNotExists(userManager, new ApplicationUser
        {
            UserName = "handyman3@helpme.bg",
            Email = "handyman3@helpme.bg",
            FirstName = "Stoyan",
            LastName = "Hristov",
            PhoneNumber = "0888000005",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Handyman");
    }

    private static async Task CreateIfNotExists(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string role)
    {
        if (await userManager.FindByEmailAsync(user.Email!) is not null)
            return;

        await userManager.CreateAsync(user, DefaultPassword);
        await userManager.AddToRoleAsync(user, role);
    }
}
