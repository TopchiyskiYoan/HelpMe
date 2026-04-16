using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HelpMe.Tests.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private Mock<RoleManager<IdentityRole>> _roleManagerMock = null!;
    private Mock<ITokenService> _tokenServiceMock = null!;
    private AuthService _authService = null!;

    [SetUp]
    public void SetUp()
    {
        var userStore = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStore.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        var roleStore = new Mock<IRoleStore<IdentityRole>>();
        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStore.Object, null!, null!, null!, null!);

        _tokenServiceMock = new Mock<ITokenService>();

        _authService = new AuthService(
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _tokenServiceMock.Object);
    }

    // --- RegisterAsync ---

    [Test]
    public async Task RegisterAsync_WithValidData_ReturnsSuccessWithToken()
    {
        var dto = new RegisterDto
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@test.bg",
            PhoneNumber = "0888123456",
            Password = "Test1234!",
            Role = "Client"
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _roleManagerMock.Setup(m => m.RoleExistsAsync("Client"))
            .ReturnsAsync(true);
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<ApplicationUser>(), dto.Password))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<ApplicationUser>(), "Client"))
            .ReturnsAsync(IdentityResult.Success);
        _userManagerMock.Setup(m => m.GetRolesAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(new List<string> { "Client" });
        _tokenServiceMock.Setup(m => m.GenerateToken(It.IsAny<ApplicationUser>(), It.IsAny<IList<string>>()))
            .Returns("jwt-token");

        var result = await _authService.RegisterAsync(dto);

        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data, Is.Not.Null);
        Assert.That(result.Data!.Token, Is.EqualTo("jwt-token"));
        Assert.That(result.Data.Email, Is.EqualTo("ivan@test.bg"));
        Assert.That(result.Data.Role, Is.EqualTo("Client"));
    }

    [Test]
    public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsEmailExistsError()
    {
        var dto = new RegisterDto { Email = "existing@test.bg", Role = "Client" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync(new ApplicationUser { Email = dto.Email });

        var result = await _authService.RegisterAsync(dto);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("EMAIL_EXISTS"));
    }

    [Test]
    public async Task RegisterAsync_WithInvalidRole_ReturnsInvalidRoleError()
    {
        var dto = new RegisterDto { Email = "ivan@test.bg", Role = "SuperAdmin" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);
        _roleManagerMock.Setup(m => m.RoleExistsAsync("SuperAdmin"))
            .ReturnsAsync(false);

        var result = await _authService.RegisterAsync(dto);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ROLE"));
    }

    [Test]
    public async Task RegisterAsync_WithAdministratorRole_ReturnsInvalidRoleError()
    {
        var dto = new RegisterDto
        {
            Email = "bad@test.bg",
            Role = "Administrator",
            FirstName = "X",
            LastName = "Y",
            PhoneNumber = "0888123456",
            Password = "Test1234!"
        };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.RegisterAsync(dto);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("INVALID_ROLE"));
    }

    // --- LoginAsync ---

    [Test]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessWithToken()
    {
        var dto = new LoginDto { Email = "ivan@test.bg", Password = "Test1234!" };
        var user = new ApplicationUser { Id = "user-1", Email = dto.Email, FirstName = "Ivan", LastName = "Ivanov" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(true);
        _userManagerMock.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Client" });
        _tokenServiceMock.Setup(m => m.GenerateToken(user, It.IsAny<IList<string>>())).Returns("jwt-token");

        var result = await _authService.LoginAsync(dto);

        Assert.That(result.Succeeded, Is.True);
        Assert.That(result.Data!.Token, Is.EqualTo("jwt-token"));
    }

    [Test]
    public async Task LoginAsync_WithWrongPassword_ReturnsInvalidCredentialsError()
    {
        var dto = new LoginDto { Email = "ivan@test.bg", Password = "WrongPass!" };
        var user = new ApplicationUser { Email = dto.Email };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.CheckPasswordAsync(user, dto.Password)).ReturnsAsync(false);

        var result = await _authService.LoginAsync(dto);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("INVALID_CREDENTIALS"));
    }

    [Test]
    public async Task LoginAsync_WithNonExistentEmail_ReturnsInvalidCredentialsError()
    {
        var dto = new LoginDto { Email = "nobody@test.bg", Password = "Test1234!" };

        _userManagerMock.Setup(m => m.FindByEmailAsync(dto.Email))
            .ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.LoginAsync(dto);

        Assert.That(result.Succeeded, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo("INVALID_CREDENTIALS"));
    }

    // --- GetCurrentUserAsync ---

    [Test]
    public async Task GetCurrentUserAsync_WhenUserExists_ReturnsUserDto()
    {
        var user = new ApplicationUser
        {
            Id = "user-1",
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@test.bg"
        };

        _userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);

        var result = await _authService.GetCurrentUserAsync("user-1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo("user-1"));
        Assert.That(result.FirstName, Is.EqualTo("Ivan"));
    }

    [Test]
    public async Task GetCurrentUserAsync_WhenUserNotFound_ReturnsNull()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing")).ReturnsAsync((ApplicationUser?)null);

        var result = await _authService.GetCurrentUserAsync("missing");

        Assert.That(result, Is.Null);
    }
}
