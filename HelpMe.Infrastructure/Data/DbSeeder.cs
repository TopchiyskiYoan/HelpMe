using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data.Seeds;
using Microsoft.AspNetCore.Identity;

namespace HelpMe.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IApplicationDbContext context)
    {
        await RoleSeed.SeedAsync(roleManager);
        await UserSeed.SeedAsync(userManager);
        await CategorySeed.SeedAsync(context);
        await LocationSeed.SeedAsync(context);
        await HandymanSeed.SeedAsync(userManager, context);
    }
}
