using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<ServiceCategory> ServiceCategories => Set<ServiceCategory>();
    public DbSet<ServiceSubCategory> ServiceSubCategories => Set<ServiceSubCategory>();
    public DbSet<Region> Regions => Set<Region>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<HandymanProfile> HandymanProfiles => Set<HandymanProfile>();
    public DbSet<HandymanSubCategory> HandymanSubCategories => Set<HandymanSubCategory>();
    public DbSet<HandymanCity> HandymanCities => Set<HandymanCity>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>().ToTable("Users");
        builder.Entity<IdentityRole>().ToTable("Roles");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

        builder.Entity<ServiceSubCategory>()
            .HasOne(s => s.Category)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<City>()
            .HasOne(c => c.Region)
            .WithMany(r => r.Cities)
            .HasForeignKey(c => c.RegionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<HandymanProfile>()
            .HasKey(h => h.UserId);

        builder.Entity<HandymanProfile>()
            .HasOne(h => h.User)
            .WithOne()
            .HasForeignKey<HandymanProfile>(h => h.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<HandymanSubCategory>()
            .HasKey(hs => new { hs.UserId, hs.SubCategoryId });

        builder.Entity<HandymanSubCategory>()
            .HasOne(hs => hs.HandymanProfile)
            .WithMany(h => h.SubCategories)
            .HasForeignKey(hs => hs.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<HandymanSubCategory>()
            .HasOne(hs => hs.SubCategory)
            .WithMany()
            .HasForeignKey(hs => hs.SubCategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<HandymanCity>()
            .HasKey(hc => new { hc.UserId, hc.CityId });

        builder.Entity<HandymanCity>()
            .HasOne(hc => hc.HandymanProfile)
            .WithMany(h => h.Cities)
            .HasForeignKey(hc => hc.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<HandymanCity>()
            .HasOne(hc => hc.City)
            .WithMany()
            .HasForeignKey(hc => hc.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
