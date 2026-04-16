using Microsoft.AspNetCore.Identity;

namespace HelpMe.Infrastructure.Data.Seeds;

public static class RoleSeed
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = ["Administrator", "Client", "Handyman"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}
