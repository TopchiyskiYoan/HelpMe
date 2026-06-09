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

    public async Task<AdminStatsDto> GetStatsAsync()
    {
        var allUsers = await _userManager.Users.ToListAsync();
        var clients = new List<ApplicationUser>();
        var handymen = new List<ApplicationUser>();
        foreach (var u in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(u);
            if (roles.Contains("Client")) clients.Add(u);
            else if (roles.Contains("Handyman")) handymen.Add(u);
        }

        var handymanProfiles = await _context.HandymanProfiles.ToListAsync();
        var verified = handymanProfiles.Count(h => h.IsVerified);
        var pending = handymanProfiles.Count(h => !h.IsVerified);

        var jobs = await _context.Jobs.ToListAsync();
        var reviews = await _context.Reviews.ToListAsync();
        var avgRating = reviews.Count > 0 ? reviews.Average(r => r.Rating) : 0;

        return new AdminStatsDto
        {
            TotalUsers = allUsers.Count,
            TotalClients = clients.Count,
            TotalHandymen = handymen.Count,
            VerifiedHandymen = verified,
            PendingVerifications = pending,
            TotalJobs = jobs.Count,
            OpenJobs = jobs.Count(j => j.Status == JobStatus.Open),
            InProgressJobs = jobs.Count(j => j.Status == JobStatus.InProgress),
            CompletedJobs = jobs.Count(j => j.Status == JobStatus.Completed),
            TotalReviews = reviews.Count,
            AverageRating = Math.Round(avgRating, 1)
        };
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
                PhoneNumber = user.PhoneNumber,
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

    public async Task<AdminUserDetailDto?> GetUserDetailAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;

        var dto = new AdminUserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            Role = role,
            IsBanned = user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow,
            CreatedAt = user.CreatedAt
        };

        if (role == "Handyman")
        {
            var profile = await _context.HandymanProfiles
                .Include(h => h.SubCategories).ThenInclude(sc => sc.SubCategory)
                .Include(h => h.Cities).ThenInclude(c => c.City)
                .FirstOrDefaultAsync(h => h.UserId == id);

            if (profile is not null)
            {
                dto.AverageRating = profile.AverageRating;
                dto.ReviewCount = profile.ReviewCount;
                dto.IsVerified = profile.IsVerified;
                dto.YearsOfExperience = profile.YearsOfExperience;
                dto.SubCategories = profile.SubCategories
                    .Where(sc => sc.SubCategory is not null)
                    .Select(sc => sc.SubCategory!.Name)
                    .ToList();
                dto.Cities = profile.Cities
                    .Where(c => c.City is not null)
                    .Select(c => c.City!.Name)
                    .ToList();
            }
        }
        else if (role == "Client")
        {
            dto.TotalJobs = await _context.Jobs.CountAsync(j => j.ClientId == id);
            dto.CompletedJobs = await _context.Jobs.CountAsync(j => j.ClientId == id && j.Status == JobStatus.Completed);
        }

        return dto;
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

    public async Task<PagedResult<AdminJobDto>> GetAllJobsAsync(string? status, int page, string? sortBy = null, string? sortDir = "desc", int pageSize = 10)
    {
        var query = _context.Jobs
            .Include(j => j.Client)
            .Include(j => j.SubCategory)
            .Include(j => j.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<JobStatus>(status, out var jobStatus))
            query = query.Where(j => j.Status == jobStatus);

        var totalCount = await query.CountAsync();

        query = (sortBy?.ToLower(), sortDir?.ToLower() == "asc") switch
        {
            ("title", true)  => query.OrderBy(j => j.Title),
            ("title", false) => query.OrderByDescending(j => j.Title),
            ("budget", true)  => query.OrderBy(j => j.ApproximateBudget),
            ("budget", false) => query.OrderByDescending(j => j.ApproximateBudget),
            ("status", true)  => query.OrderBy(j => j.Status),
            ("status", false) => query.OrderByDescending(j => j.Status),
            (_, true)  => query.OrderBy(j => j.CreatedAt),
            _          => query.OrderByDescending(j => j.CreatedAt),
        };

        var jobs = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

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

    public async Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(int page, string? sortBy = null, string? sortDir = "desc", int pageSize = 10)
    {
        var query = _context.Reviews
            .Include(r => r.Client)
            .Include(r => r.Handyman)
                .ThenInclude(h => h.User)
            .AsQueryable();

        var totalCount = await query.CountAsync();

        query = (sortBy?.ToLower(), sortDir?.ToLower() == "asc") switch
        {
            ("rating", true)  => query.OrderBy(r => r.Rating),
            ("rating", false) => query.OrderByDescending(r => r.Rating),
            ("handyman", true)  => query.OrderBy(r => r.Handyman!.User!.FirstName),
            ("handyman", false) => query.OrderByDescending(r => r.Handyman!.User!.FirstName),
            (_, true)  => query.OrderBy(r => r.CreatedAt),
            _          => query.OrderByDescending(r => r.CreatedAt),
        };

        var reviews = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

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
