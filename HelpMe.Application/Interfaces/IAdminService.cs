using HelpMe.Application.DTOs;
using HelpMe.Domain.Entities;

namespace HelpMe.Application.Interfaces;

public interface IAdminService
{
    Task<AdminStatsDto> GetStatsAsync();
    Task<PagedResult<AdminUserDto>> GetAllUsersAsync(string? search, int page, int pageSize = 10);
    Task<AdminUserDetailDto?> GetUserDetailAsync(string id);
    Task<bool> BanUserAsync(string userId);
    Task<bool> UnbanUserAsync(string userId);
    Task<PagedResult<AdminJobDto>> GetAllJobsAsync(string? status, int page, string? sortBy = null, string? sortDir = "desc", int pageSize = 10);
    Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(int page, string? sortBy = null, string? sortDir = "desc", int pageSize = 10);
}
