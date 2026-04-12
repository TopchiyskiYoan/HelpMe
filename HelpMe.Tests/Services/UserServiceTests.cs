using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace HelpMe.Tests.Services;

[TestFixture]
public class UserServiceTests
{
    private Mock<UserManager<ApplicationUser>> _userManagerMock = null!;
    private UserService _userService = null!;

    [SetUp]
    public void SetUp()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

        _userService = new UserService(_userManagerMock.Object);
    }

    // --- GetUserByIdAsync ---

    [Test]
    public async Task GetUserByIdAsync_WhenUserExists_ReturnsUserDto()
    {
        var user = new ApplicationUser
        {
            Id = "user-1",
            FirstName = "Ivan",
            LastName = "Ivanov",
            Email = "ivan@test.bg",
            PhoneNumber = "0888123456",
            ProfilePictureUrl = null,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };

        _userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);

        var result = await _userService.GetUserByIdAsync("user-1");

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo("user-1"));
        Assert.That(result.FirstName, Is.EqualTo("Ivan"));
        Assert.That(result.LastName, Is.EqualTo("Ivanov"));
        Assert.That(result.Email, Is.EqualTo("ivan@test.bg"));
        Assert.That(result.PhoneNumber, Is.EqualTo("0888123456"));
    }

    [Test]
    public async Task GetUserByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing-id")).ReturnsAsync((ApplicationUser?)null);

        var result = await _userService.GetUserByIdAsync("missing-id");

        Assert.That(result, Is.Null);
    }

    // --- UpdateProfileAsync ---

    [Test]
    public async Task UpdateProfileAsync_WhenUserExists_UpdatesFieldsAndReturnsTrue()
    {
        var user = new ApplicationUser
        {
            Id = "user-1",
            FirstName = "Old",
            LastName = "Name",
            PhoneNumber = "0888000000"
        };

        var dto = new UpdateProfileDto
        {
            FirstName = "Ivan",
            LastName = "Ivanov",
            PhoneNumber = "0888123456",
            ProfilePictureUrl = "/uploads/avatars/user-1.jpg"
        };

        _userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Success);

        var result = await _userService.UpdateProfileAsync("user-1", dto);

        Assert.That(result, Is.True);
        Assert.That(user.FirstName, Is.EqualTo("Ivan"));
        Assert.That(user.LastName, Is.EqualTo("Ivanov"));
        Assert.That(user.PhoneNumber, Is.EqualTo("0888123456"));
        Assert.That(user.ProfilePictureUrl, Is.EqualTo("/uploads/avatars/user-1.jpg"));
    }

    [Test]
    public async Task UpdateProfileAsync_WhenUserDoesNotExist_ReturnsFalse()
    {
        _userManagerMock.Setup(m => m.FindByIdAsync("missing-id")).ReturnsAsync((ApplicationUser?)null);

        var result = await _userService.UpdateProfileAsync("missing-id", new UpdateProfileDto());

        Assert.That(result, Is.False);
    }

    [Test]
    public async Task UpdateProfileAsync_WhenUpdateFails_ReturnsFalse()
    {
        var user = new ApplicationUser { Id = "user-1" };

        _userManagerMock.Setup(m => m.FindByIdAsync("user-1")).ReturnsAsync(user);
        _userManagerMock.Setup(m => m.UpdateAsync(It.IsAny<ApplicationUser>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "DB error" }));

        var result = await _userService.UpdateProfileAsync("user-1", new UpdateProfileDto());

        Assert.That(result, Is.False);
    }
}
