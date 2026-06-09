using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IApplicationDbContext _context;

    public AdminService(UserManager<ApplicationUser> userManager, IApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<PagedResult<AdminUserDto>> GetAllUsersAsync(string? search, int page, int pageSize = 10)
    {
        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(u =>
                u.Email!.ToLower().Contains(lower) ||
                u.FirstName.ToLower().Contains(lower) ||
                u.LastName.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync();
        var users = await query
            .OrderBy(u => u.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var dtos = new List<AdminUserDto>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            dtos.Add(new AdminUserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email ?? string.Empty,
                Role = roles.FirstOrDefault() ?? string.Empty,
                IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
                CreatedAt = user.CreatedAt
            });
        }

        return new PagedResult<AdminUserDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<bool> BanUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        await _userManager.SetLockoutEnabledAsync(user, true);
        await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
        return true;
    }

    public async Task<bool> UnbanUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return false;

        await _userManager.SetLockoutEndDateAsync(user, null);
        return true;
    }

    public async Task<PagedResult<AdminJobDto>> GetAllJobsAsync(string? status, int page, int pageSize = 10)
    {
        var query = _context.Jobs
            .Include(j => j.Client)
            .Include(j => j.SubCategory)
            .Include(j => j.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<JobStatus>(status, out var jobStatus))
            query = query.Where(j => j.Status == jobStatus);

        var totalCount = await query.CountAsync();
        var jobs = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<AdminJobDto>
        {
            Items = jobs.Select(j => new AdminJobDto
            {
                Id = j.Id,
                Title = j.Title,
                ClientName = j.Client is not null ? $"{j.Client.FirstName} {j.Client.LastName}" : string.Empty,
                SubCategoryName = j.SubCategory?.Name ?? string.Empty,
                CityName = j.City?.Name ?? string.Empty,
                Status = j.Status.ToString(),
                CreatedAt = j.CreatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(int page, int pageSize = 10)
    {
        var query = _context.Reviews
            .Include(r => r.Client)
            .Include(r => r.Handyman)
                .ThenInclude(h => h.User)
            .AsQueryable();

        var totalCount = await query.CountAsync();
        var reviews = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<AdminReviewDto>
        {
            Items = reviews.Select(r => new AdminReviewDto
            {
                Id = r.Id,
                ClientName = r.Client is not null ? $"{r.Client.FirstName} {r.Client.LastName}" : string.Empty,
                HandymanName = r.Handyman?.User is not null ? $"{r.Handyman.User.FirstName} {r.Handyman.User.LastName}" : string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt
            }).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
}
