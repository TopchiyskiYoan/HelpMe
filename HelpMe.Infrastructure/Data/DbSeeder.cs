using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpMe.Infrastructure.Data;

public static class DbSeeder
{
    private const string DefaultPassword = "Test1234!";

    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await SeedRolesAsync(roleManager);
        await SeedUsersAsync(userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Administrator", "Client", "Handyman"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "admin@helpme.bg",
            Email = "admin@helpme.bg",
            FirstName = "Admin",
            LastName = "HelpMe",
            PhoneNumber = "0888000001",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Administrator");

        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "client@helpme.bg",
            Email = "client@helpme.bg",
            FirstName = "Georgi",
            LastName = "Petrov",
            PhoneNumber = "0888000002",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Client");

        await CreateUserIfNotExists(userManager, new ApplicationUser
        {
            UserName = "handyman@helpme.bg",
            Email = "handyman@helpme.bg",
            FirstName = "Dimitar",
            LastName = "Kolev",
            PhoneNumber = "0888000003",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        }, "Handyman");
    }

    private static async Task CreateUserIfNotExists(
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
