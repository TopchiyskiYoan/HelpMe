using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResult> RegisterAsync(RegisterDto dto);
    Task<AuthResult> LoginAsync(LoginDto dto);
    Task<UserDto?> GetCurrentUserAsync(string userId);
}
