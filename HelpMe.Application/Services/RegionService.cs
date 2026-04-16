using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class RegionService : IRegionService
{
    private readonly IApplicationDbContext _context;

    public RegionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<RegionDto>> GetAllAsync()
    {
        var regions = await _context.Regions
            .Where(r => r.IsActive)
            .Include(r => r.Cities)
            .OrderBy(r => r.Name)
            .ToListAsync();

        return regions.Select(ToDto).ToList();
    }

    public async Task<RegionDto?> GetByIdAsync(int id)
    {
        var region = await _context.Regions
            .Where(r => r.Id == id && r.IsActive)
            .Include(r => r.Cities)
            .FirstOrDefaultAsync();

        return region is null ? null : ToDto(region);
    }

    private static RegionDto ToDto(Domain.Entities.Region r) => new()
    {
        Id = r.Id,
        Name = r.Name,
        Cities = r.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList()
    };
}
