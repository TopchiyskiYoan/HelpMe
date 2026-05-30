using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<ServiceSubCategory> ServiceSubCategories { get; }
    DbSet<Region> Regions { get; }
    DbSet<City> Cities { get; }
    DbSet<HandymanProfile> HandymanProfiles { get; }
    DbSet<HandymanSubCategory> HandymanSubCategories { get; }
    DbSet<HandymanCity> HandymanCities { get; }
    DbSet<Job> Jobs { get; }
    DbSet<JobInterest> JobInterests { get; }
    DbSet<Review> Reviews { get; }
    DbSet<ReviewResponse> ReviewResponses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
