using System.Security.Claims;
using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace HelpMe.Tests.Controllers;

[TestFixture]
public class HandymanProfilesControllerTests
{
    private Mock<IHandymanProfileService> _serviceMock = null!;
    private HandymanProfilesController _controller = null!;

    [SetUp]
    public void SetUp()
    {
        _serviceMock = new Mock<IHandymanProfileService>();
        _controller = new HandymanProfilesController(_serviceMock.Object);
    }

    private static ControllerContext WithUser(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
        return new ControllerContext { HttpContext = new DefaultHttpContext { User = principal } };
    }

    private static ControllerContext WithNoUser() =>
        new() { HttpContext = new DefaultHttpContext() };

    private static HandymanProfileDto BuildProfileDto(string userId) => new()
    {
        UserId = userId,
        FirstName = "Иван",
        LastName = "Майсторов",
        Bio = "Опитен майстор",
        YearsOfExperience = 5,
        IsActive = true,
        IsVerified = true,
        SubCategories = [new SubCategoryDto { Id = 1, Name = "Течове" }],
        Cities = [new CityDto { Id = 1, Name = "София" }]
    };

    // --- GetAll ---

    [Test]
    public async Task GetAll_ReturnsOkWithList()
    {
        var profiles = new List<HandymanProfileDto>
        {
            BuildProfileDto("user-1"),
            BuildProfileDto("user-2")
        };

        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(profiles);

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(profiles));
    }

    [Test]
    public async Task GetAll_WhenEmpty_ReturnsOkWithEmptyList()
    {
        _serviceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<HandymanProfileDto>());

        var result = await _controller.GetAll();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var list = ok!.Value as List<HandymanProfileDto>;
        Assert.That(list, Is.Empty);
    }

    // --- GetByUserId (public profile) ---

    [Test]
    public async Task GetByUserId_WhenVerifiedProfile_ReturnsOk()
    {
        var profile = BuildProfileDto("user-1");

        _serviceMock.Setup(s => s.GetPublicProfileAsync("user-1")).ReturnsAsync(profile);

        var result = await _controller.GetByUserId("user-1");

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(profile));
    }

    [Test]
    public async Task GetByUserId_WhenNotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.GetPublicProfileAsync("user-1")).ReturnsAsync((HandymanProfileDto?)null);

        var result = await _controller.GetByUserId("user-1");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // --- GetMe ---

    [Test]
    public async Task GetMe_WhenProfileExists_ReturnsOk()
    {
        _controller.ControllerContext = WithUser("user-1");
        var profile = BuildProfileDto("user-1");

        _serviceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync(profile);

        var result = await _controller.GetMe();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(profile));
    }

    [Test]
    public async Task GetMe_WhenProfileNotFound_ReturnsNotFound()
    {
        _controller.ControllerContext = WithUser("user-1");

        _serviceMock.Setup(s => s.GetByUserIdAsync("user-1")).ReturnsAsync((HandymanProfileDto?)null);

        var result = await _controller.GetMe();

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task GetMe_WhenNoUserId_ReturnsUnauthorized()
    {
        _controller.ControllerContext = WithNoUser();

        var result = await _controller.GetMe();

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    // --- Create ---

    [Test]
    public async Task Create_WhenSuccess_ReturnsCreatedAtAction()
    {
        _controller.ControllerContext = WithUser("user-1");
        var dto = new CreateHandymanProfileDto
        {
            Bio = "Нов майстор",
            YearsOfExperience = 3,
            SubCategoryIds = [1],
            CityIds = [1]
        };
        var created = BuildProfileDto("user-1");

        _serviceMock.Setup(s => s.CreateAsync("user-1", dto)).ReturnsAsync(created);

        var result = await _controller.Create(dto);

        var createdAt = result as CreatedAtActionResult;
        Assert.That(createdAt, Is.Not.Null);
        Assert.That(createdAt!.Value, Is.EqualTo(created));
        Assert.That(createdAt.RouteValues!["userId"], Is.EqualTo(created.UserId));
    }

    [Test]
    public async Task Create_WhenProfileAlreadyExists_ReturnsConflict()
    {
        _controller.ControllerContext = WithUser("user-1");
        var dto = new CreateHandymanProfileDto { SubCategoryIds = [1], CityIds = [1] };

        _serviceMock.Setup(s => s.CreateAsync("user-1", dto)).ReturnsAsync((HandymanProfileDto?)null);

        var result = await _controller.Create(dto);

        Assert.That(result, Is.InstanceOf<ConflictObjectResult>());
    }

    [Test]
    public async Task Create_WhenNoUserId_ReturnsUnauthorized()
    {
        _controller.ControllerContext = WithNoUser();
        var dto = new CreateHandymanProfileDto { SubCategoryIds = [1], CityIds = [1] };

        var result = await _controller.Create(dto);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    // --- Update ---

    [Test]
    public async Task Update_WhenSuccess_ReturnsNoContent()
    {
        _controller.ControllerContext = WithUser("user-1");
        var dto = new UpdateHandymanProfileDto
        {
            Bio = "Обновена биография",
            YearsOfExperience = 10,
            SubCategoryIds = [1],
            CityIds = [1]
        };

        _serviceMock.Setup(s => s.UpdateAsync("user-1", dto)).ReturnsAsync(true);

        var result = await _controller.Update(dto);

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Update_WhenProfileNotFound_ReturnsNotFound()
    {
        _controller.ControllerContext = WithUser("user-1");
        var dto = new UpdateHandymanProfileDto { SubCategoryIds = [1], CityIds = [1] };

        _serviceMock.Setup(s => s.UpdateAsync("user-1", dto)).ReturnsAsync(false);

        var result = await _controller.Update(dto);

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    [Test]
    public async Task Update_WhenNoUserId_ReturnsUnauthorized()
    {
        _controller.ControllerContext = WithNoUser();
        var dto = new UpdateHandymanProfileDto { SubCategoryIds = [1], CityIds = [1] };

        var result = await _controller.Update(dto);

        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    // --- Deactivate (Admin) ---

    [Test]
    public async Task Deactivate_WhenSuccess_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.DeactivateAsync("user-1")).ReturnsAsync(true);

        var result = await _controller.Deactivate("user-1");

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Deactivate_WhenNotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.DeactivateAsync("nonexistent")).ReturnsAsync(false);

        var result = await _controller.Deactivate("nonexistent");

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }

    // --- GetPending (Admin) ---

    [Test]
    public async Task GetPending_ReturnsOkWithPendingList()
    {
        var pending = new List<HandymanProfileDto>
        {
            new() { UserId = "user-1", IsActive = true, IsVerified = false }
        };

        _serviceMock.Setup(s => s.GetPendingVerificationAsync()).ReturnsAsync(pending);

        var result = await _controller.GetPending();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        Assert.That(ok!.Value, Is.EqualTo(pending));
    }

    [Test]
    public async Task GetPending_WhenNoPending_ReturnsOkWithEmptyList()
    {
        _serviceMock.Setup(s => s.GetPendingVerificationAsync()).ReturnsAsync(new List<HandymanProfileDto>());

        var result = await _controller.GetPending();

        var ok = result as OkObjectResult;
        Assert.That(ok, Is.Not.Null);
        var list = ok!.Value as List<HandymanProfileDto>;
        Assert.That(list, Is.Empty);
    }

    // --- Verify (Admin) ---

    [Test]
    public async Task Verify_WhenApproved_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.VerifyAsync("user-1", true)).ReturnsAsync(true);

        var result = await _controller.Verify("user-1", new VerifyHandymanDto { Approved = true });

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Verify_WhenRejected_ReturnsNoContent()
    {
        _serviceMock.Setup(s => s.VerifyAsync("user-1", false)).ReturnsAsync(true);

        var result = await _controller.Verify("user-1", new VerifyHandymanDto { Approved = false });

        Assert.That(result, Is.InstanceOf<NoContentResult>());
    }

    [Test]
    public async Task Verify_WhenNotFound_ReturnsNotFound()
    {
        _serviceMock.Setup(s => s.VerifyAsync("nonexistent", true)).ReturnsAsync(false);

        var result = await _controller.Verify("nonexistent", new VerifyHandymanDto { Approved = true });

        Assert.That(result, Is.InstanceOf<NotFoundResult>());
    }
}
