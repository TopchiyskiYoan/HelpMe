using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class HandymanProfileService : IHandymanProfileService
{
    private readonly IApplicationDbContext _context;

    public HandymanProfileService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HandymanProfileDto>> GetAllAsync()
    {
        var profiles = await _context.HandymanProfiles
            .Where(h => h.IsActive)
            .Include(h => h.User)
            .Include(h => h.SubCategories)
                .ThenInclude(hs => hs.SubCategory)
            .Include(h => h.Cities)
                .ThenInclude(hc => hc.City)
            .OrderBy(h => h.User.FirstName)
            .ToListAsync();

        return profiles.Select(ToDto).ToList();
    }

    public async Task<HandymanProfileDto?> GetByUserIdAsync(string userId)
    {
        var profile = await _context.HandymanProfiles
            .Where(h => h.UserId == userId && h.IsActive)
            .Include(h => h.User)
            .Include(h => h.SubCategories)
                .ThenInclude(hs => hs.SubCategory)
            .Include(h => h.Cities)
                .ThenInclude(hc => hc.City)
            .FirstOrDefaultAsync();

        return profile is null ? null : ToDto(profile);
    }

    public async Task<HandymanProfileDto> CreateAsync(string userId, CreateHandymanProfileDto dto)
    {
        var profile = new HandymanProfile
        {
            UserId = userId,
            Bio = dto.Bio?.Trim(),
            YearsOfExperience = dto.YearsOfExperience,
            SubCategories = dto.SubCategoryIds
                .Select(id => new HandymanSubCategory { UserId = userId, SubCategoryId = id })
                .ToList(),
            Cities = dto.CityIds
                .Select(id => new HandymanCity { UserId = userId, CityId = id })
                .ToList()
        };

        await _context.HandymanProfiles.AddAsync(profile);
        await _context.SaveChangesAsync();

        return await GetByUserIdAsync(userId) ?? ToDto(profile);
    }

    public async Task<bool> UpdateAsync(string userId, UpdateHandymanProfileDto dto)
    {
        var profile = await _context.HandymanProfiles
            .Include(h => h.SubCategories)
            .Include(h => h.Cities)
            .FirstOrDefaultAsync(h => h.UserId == userId && h.IsActive);

        if (profile is null) return false;

        profile.Bio = dto.Bio?.Trim();
        profile.YearsOfExperience = dto.YearsOfExperience;

        _context.HandymanSubCategories.RemoveRange(profile.SubCategories);
        _context.HandymanCities.RemoveRange(profile.Cities);

        profile.SubCategories = dto.SubCategoryIds
            .Select(id => new HandymanSubCategory { UserId = userId, SubCategoryId = id })
            .ToList();

        profile.Cities = dto.CityIds
            .Select(id => new HandymanCity { UserId = userId, CityId = id })
            .ToList();

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(string userId)
    {
        var profile = await _context.HandymanProfiles
            .FirstOrDefaultAsync(h => h.UserId == userId);

        if (profile is null) return false;

        profile.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    private static HandymanProfileDto ToDto(HandymanProfile h) => new()
    {
        UserId = h.UserId,
        FirstName = h.User?.FirstName ?? string.Empty,
        LastName = h.User?.LastName ?? string.Empty,
        Bio = h.Bio,
        YearsOfExperience = h.YearsOfExperience,
        IsActive = h.IsActive,
        SubCategories = h.SubCategories
            .Where(hs => hs.SubCategory is not null)
            .Select(hs => new SubCategoryDto
            {
                Id = hs.SubCategory.Id,
                Name = hs.SubCategory.Name,
                Description = hs.SubCategory.Description,
                IsActive = hs.SubCategory.IsActive
            }).ToList(),
        Cities = h.Cities
            .Where(hc => hc.City is not null)
            .Select(hc => new CityDto
            {
                Id = hc.City.Id,
                Name = hc.City.Name
            }).ToList()
    };
}
