using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<bool> UpdateProfileAsync(string id, UpdateProfileDto dto);
}
