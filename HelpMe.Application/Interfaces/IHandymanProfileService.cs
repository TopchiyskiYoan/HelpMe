using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IHandymanProfileService
{
    Task<List<HandymanProfileDto>> GetAllAsync();
    Task<HandymanProfileDto?> GetByUserIdAsync(string userId);
    Task<HandymanProfileDto?> GetPublicProfileAsync(string userId);
    Task<HandymanProfileDto?> CreateAsync(string userId, CreateHandymanProfileDto dto);
    Task<bool> UpdateAsync(string userId, UpdateHandymanProfileDto dto);
    Task<bool> DeactivateAsync(string userId);
    Task<List<HandymanProfileDto>> GetPendingVerificationAsync();
    Task<bool> VerifyAsync(string userId, bool approved);
}
