using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _context.ServiceCategories
            .Where(c => c.IsActive)
            .Include(c => c.SubCategories)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return categories.Select(c => ToDto(c)).ToList();
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var category = await _context.ServiceCategories
            .Where(c => c.Id == id && c.IsActive)
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync();

        return category is null ? null : ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var name = dto.Name.Trim();
        var category = new ServiceCategory { Name = name, IsActive = true };

        _context.ServiceCategories.Add(category);
        await _context.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<bool> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _context.ServiceCategories.FindAsync(id);
        if (category is null) return false;

        category.Name = dto.Name.Trim();
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var category = await _context.ServiceCategories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (category is null) return false;

        category.IsActive = false;
        foreach (var sub in category.SubCategories)
            sub.IsActive = false;

        await _context.SaveChangesAsync();

        return true;
    }

    private static CategoryDto ToDto(ServiceCategory c) => new()
    {
        Id = c.Id,
        Name = c.Name,
        IsActive = c.IsActive,
        SubCategories = c.SubCategories.Where(s => s.IsActive).Select(s => new SubCategoryDto
        {
            Id = s.Id,
            Name = s.Name,
            Description = s.Description,
            IsActive = s.IsActive
        }).ToList()
    };
}
