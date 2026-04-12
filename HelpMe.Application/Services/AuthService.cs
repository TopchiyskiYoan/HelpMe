using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace HelpMe.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _tokenService = tokenService;
    }

    public async Task<AuthResult> RegisterAsync(RegisterDto dto)
    {
        if (await _userManager.FindByEmailAsync(dto.Email) is not null)
            return AuthResult.Fail("EMAIL_EXISTS");

        if (!await _roleManager.RoleExistsAsync(dto.Role))
            return AuthResult.Fail("INVALID_ROLE");

        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(user, dto.Password);
        if (!createResult.Succeeded)
            return AuthResult.Fail("CREATE_FAILED");

        await _userManager.AddToRoleAsync(user, dto.Role);

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return AuthResult.Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? string.Empty
        });
    }

    public async Task<AuthResult> LoginAsync(LoginDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return AuthResult.Fail("INVALID_CREDENTIALS");

        if (!await _userManager.CheckPasswordAsync(user, dto.Password))
            return AuthResult.Fail("INVALID_CREDENTIALS");

        var roles = await _userManager.GetRolesAsync(user);
        var token = _tokenService.GenerateToken(user, roles);

        return AuthResult.Ok(new AuthResponseDto
        {
            Token = token,
            UserId = user.Id,
            Email = user.Email!,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Role = roles.FirstOrDefault() ?? string.Empty
        });
    }

    public async Task<UserDto?> GetCurrentUserAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null) return null;

        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            ProfilePictureUrl = user.ProfilePictureUrl,
            CreatedAt = user.CreatedAt
        };
    }
}
