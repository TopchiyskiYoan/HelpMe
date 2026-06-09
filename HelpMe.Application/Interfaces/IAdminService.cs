using HelpMe.Application.DTOs;
using HelpMe.Domain.Entities;

namespace HelpMe.Application.Interfaces;

public interface IAdminService
{
    Task<PagedResult<AdminUserDto>> GetAllUsersAsync(string? search, int page, int pageSize = 10);
    Task<bool> BanUserAsync(string userId);
    Task<bool> UnbanUserAsync(string userId);
    Task<PagedResult<AdminJobDto>> GetAllJobsAsync(string? status, int page, int pageSize = 10);
    Task<PagedResult<AdminReviewDto>> GetAllReviewsAsync(int page, int pageSize = 10);
}
