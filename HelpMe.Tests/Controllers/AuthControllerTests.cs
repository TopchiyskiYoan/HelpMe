using System.Security.Claims;
using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HelpMe.Tests.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private Mock<IAuthService> _authServiceMock = null!;
    private AuthController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    private static ControllerContext WithUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };
    }

    private static ControllerContext WithNoUser() =>
        new() { HttpContext = new DefaultHttpContext() };

    // --- Register ---

    [Test]
    public async Task Register_WhenSuccess_ReturnsOkWithToken()
    {
        var dto = new RegisterDto { Email = "ivan@test.bg", Role = "Client" };
        var response = new AuthResponseDto { Token = "jwt-token", Email = "ivan@test.bg", Role = "Client" };

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(AuthResult.Ok(response));

        var result = await _controller.Register(dto);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task Register_WhenEmailExists_ReturnsConflict()
    {
        var dto = new RegisterDto { Email = "existing@test.bg", Role = "Client" };

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(AuthResult.Fail("EMAIL_EXISTS"));

        var result = await _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
    }

    [Test]
    public async Task Register_WhenInvalidRole_ReturnsBadRequest()
    {
        var dto = new RegisterDto { Email = "ivan@test.bg", Role = "SuperAdmin" };

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(AuthResult.Fail("INVALID_ROLE"));

        var result = await _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Register_WhenInvalidPhone_ReturnsBadRequest()
    {
        var dto = new RegisterDto { Email = "ivan@test.bg", Role = "Client", PhoneNumber = "123" };

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(AuthResult.Fail("INVALID_PHONE"));

        var result = await _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Register_WhenUnknownError_ReturnsBadRequest()
    {
        var dto = new RegisterDto { Email = "ivan@test.bg", Role = "Client" };

        _authServiceMock.Setup(s => s.RegisterAsync(dto))
            .ReturnsAsync(AuthResult.Fail("UNKNOWN_ERROR"));

        var result = await _controller.Register(dto);

        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
    }

    // --- Login ---

    [Test]
    public async Task Login_WhenSuccess_ReturnsOkWithToken()
    {
        var dto = new LoginDto { Email = "ivan@test.bg", Password = "Test1234!" };
        var response = new AuthResponseDto { Token = "jwt-token", Email = "ivan@test.bg" };

        _authServiceMock.Setup(s => s.LoginAsync(dto))
            .ReturnsAsync(AuthResult.Ok(response));

        var result = await _controller.Login(dto);

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(response));
    }

    [Test]
    public async Task Login_WhenInvalidCredentials_ReturnsUnauthorized()
    {
        var dto = new LoginDto { Email = "ivan@test.bg", Password = "wrong" };

        _authServiceMock.Setup(s => s.LoginAsync(dto))
            .ReturnsAsync(AuthResult.Fail("INVALID_CREDENTIALS"));

        var result = await _controller.Login(dto);

        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    // --- Me ---

    [Test]
    public async Task Me_WhenUserExists_ReturnsOkWithUser()
    {
        _controller.ControllerContext = WithUser("user-1");
        var userDto = new UserDto { Id = "user-1", FirstName = "Ivan", LastName = "Ivanov" };

        _authServiceMock.Setup(s => s.GetCurrentUserAsync("user-1"))
            .ReturnsAsync(userDto);

        var result = await _controller.Me();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(userDto));
    }

    [Test]
    public async Task Me_WhenUserNotFound_ReturnsNotFound()
    {
        _controller.ControllerContext = WithUser("user-1");

        _authServiceMock.Setup(s => s.GetCurrentUserAsync("user-1"))
            .ReturnsAsync((UserDto?)null);

        var result = await _controller.Me();

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Me_WhenNoUserId_ReturnsUnauthorized()
    {
        _controller.ControllerContext = WithNoUser();

        var result = await _controller.Me();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }
}
