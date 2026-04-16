using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<ServiceSubCategory> ServiceSubCategories { get; }
    DbSet<Region> Regions { get; }
    DbSet<City> Cities { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
