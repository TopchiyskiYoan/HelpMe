using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<bool> UpdateProfileAsync(string id, UpdateProfileDto dto);
    Task<(bool Success, string? Error)> ChangePasswordAsync(string id, ChangePasswordDto dto);
}
