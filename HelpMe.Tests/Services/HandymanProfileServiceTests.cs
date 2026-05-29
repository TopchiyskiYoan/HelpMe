using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class HandymanProfileServiceTests
{
    private ApplicationDbContext _context = null!;
    private HandymanProfileService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new HandymanProfileService(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context.Dispose();
    }

    private ApplicationUser CreateUser(string id, string firstName = "Иван", string lastName = "Иванов")
    {
        var user = new ApplicationUser
        {
            Id = id,
            UserName = $"{id}@test.bg",
            Email = $"{id}@test.bg",
            FirstName = firstName,
            LastName = lastName
        };
        _context.Users.Add(user);
        return user;
    }

    private ServiceSubCategory CreateSubCategory(string name = "Течове")
    {
        var sub = new ServiceSubCategory
        {
            Name = name,
            Description = "Описание",
            IsActive = true,
            Category = new ServiceCategory { Name = "ВиК", IsActive = true }
        };
        _context.ServiceSubCategories.Add(sub);
        return sub;
    }

    private City CreateCity(string name = "София")
    {
        var city = new City
        {
            Name = name,
            Region = new Region { Name = "София (град)" }
        };
        _context.Cities.Add(city);
        return city;
    }

    // --- GetAllAsync ---

    [Test]
    public async Task GetAllAsync_ReturnsOnlyActiveAndVerifiedProfiles()
    {
        var user1 = CreateUser("user-1", "Димитър", "Колев");
        var user2 = CreateUser("user-2", "Архив", "Потребител");
        var user3 = CreateUser("user-3", "Иван", "Неверифициран");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.AddRange(
            new HandymanProfile { UserId = user1.Id, IsActive = true, IsVerified = true },
            new HandymanProfile { UserId = user2.Id, IsActive = false, IsVerified = true },
            new HandymanProfile { UserId = user3.Id, IsActive = true, IsVerified = false }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].UserId, Is.EqualTo(user1.Id));
    }

    [Test]
    public async Task GetAllAsync_IncludesSubCategoriesAndCities()
    {
        var user = CreateUser("user-1");
        var sub = CreateSubCategory("Плочки");
        var city = CreateCity("Варна");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile
        {
            UserId = user.Id,
            IsActive = true,
            IsVerified = true,
            SubCategories = [new HandymanSubCategory { UserId = user.Id, SubCategoryId = sub.Id }],
            Cities = [new HandymanCity { UserId = user.Id, CityId = city.Id }]
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetAllAsync();

        Assert.That(result[0].SubCategories, Has.Count.EqualTo(1));
        Assert.That(result[0].SubCategories[0].Name, Is.EqualTo("Плочки"));
        Assert.That(result[0].Cities, Has.Count.EqualTo(1));
        Assert.That(result[0].Cities[0].Name, Is.EqualTo("Варна"));
    }

    // --- GetPublicProfileAsync ---

    [Test]
    public async Task GetPublicProfileAsync_WhenVerified_ReturnsProfile()
    {
        var user = CreateUser("user-1", "Иван", "Верифициран");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true, IsVerified = true });
        await _context.SaveChangesAsync();

        var result = await _service.GetPublicProfileAsync(user.Id);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public async Task GetPublicProfileAsync_WhenUnverified_ReturnsNull()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true, IsVerified = false });
        await _context.SaveChangesAsync();

        var result = await _service.GetPublicProfileAsync(user.Id);

        Assert.That(result, Is.Null);
    }

    // --- GetByUserIdAsync ---

    [Test]
    public async Task GetByUserIdAsync_WhenExists_ReturnsProfile()
    {
        var user = CreateUser("user-1", "Стоян", "Христов");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile
        {
            UserId = user.Id,
            Bio = "Опитен майстор",
            YearsOfExperience = 8,
            IsActive = true
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetByUserIdAsync(user.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Bio, Is.EqualTo("Опитен майстор"));
        Assert.That(result.YearsOfExperience, Is.EqualTo(8));
        Assert.That(result.FirstName, Is.EqualTo("Стоян"));
    }

    [Test]
    public async Task GetByUserIdAsync_WhenNotFound_ReturnsNull()
    {
        var result = await _service.GetByUserIdAsync("nonexistent");

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetByUserIdAsync_WhenInactive_ReturnsNull()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = false });
        await _context.SaveChangesAsync();

        var result = await _service.GetByUserIdAsync(user.Id);

        Assert.That(result, Is.Null);
    }

    // --- CreateAsync ---

    [Test]
    public async Task CreateAsync_WhenProfileAlreadyExists_ReturnsNull()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true });
        await _context.SaveChangesAsync();

        var dto = new CreateHandymanProfileDto
        {
            SubCategoryIds = [1],
            CityIds = [1]
        };

        var result = await _service.CreateAsync(user.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateAsync_SavesProfileAndReturnsDto()
    {
        var user = CreateUser("user-1", "Иван", "Георгиев");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var dto = new CreateHandymanProfileDto
        {
            Bio = "Нов майстор",
            YearsOfExperience = 5,
            SubCategoryIds = [sub.Id],
            CityIds = [city.Id]
        };

        var result = await _service.CreateAsync(user.Id, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Bio, Is.EqualTo("Нов майстор"));
        Assert.That(result.YearsOfExperience, Is.EqualTo(5));
        Assert.That(result.SubCategories, Has.Count.EqualTo(1));
        Assert.That(result.Cities, Has.Count.EqualTo(1));

        var inDb = await _context.HandymanProfiles.FindAsync(user.Id);
        Assert.That(inDb, Is.Not.Null);
    }

    // --- UpdateAsync ---

    [Test]
    public async Task UpdateAsync_WhenExists_UpdatesProfileAndReturnsTrue()
    {
        var user = CreateUser("user-1");
        var oldSub = CreateSubCategory("Стара");
        var newSub = CreateSubCategory("Нова");
        var city = CreateCity();
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile
        {
            UserId = user.Id,
            Bio = "Стара биография",
            YearsOfExperience = 3,
            IsActive = true,
            SubCategories = [new HandymanSubCategory { UserId = user.Id, SubCategoryId = oldSub.Id }],
            Cities = [new HandymanCity { UserId = user.Id, CityId = city.Id }]
        });
        await _context.SaveChangesAsync();

        var dto = new UpdateHandymanProfileDto
        {
            Bio = "Нова биография",
            YearsOfExperience = 10,
            SubCategoryIds = [newSub.Id],
            CityIds = [city.Id]
        };

        var success = await _service.UpdateAsync(user.Id, dto);

        Assert.That(success, Is.True);

        var updated = await _context.HandymanProfiles
            .Include(h => h.SubCategories)
            .FirstAsync(h => h.UserId == user.Id);

        Assert.That(updated.Bio, Is.EqualTo("Нова биография"));
        Assert.That(updated.YearsOfExperience, Is.EqualTo(10));
        Assert.That(updated.SubCategories, Has.Count.EqualTo(1));
        Assert.That(updated.SubCategories.First().SubCategoryId, Is.EqualTo(newSub.Id));
    }

    [Test]
    public async Task UpdateAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _service.UpdateAsync("nonexistent", new UpdateHandymanProfileDto
        {
            SubCategoryIds = [1],
            CityIds = [1]
        });

        Assert.That(success, Is.False);
    }

    // --- DeactivateAsync ---

    [Test]
    public async Task DeactivateAsync_WhenExists_SetsIsActiveFalseAndReturnsTrue()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true });
        await _context.SaveChangesAsync();

        var success = await _service.DeactivateAsync(user.Id);

        Assert.That(success, Is.True);
        var profile = await _context.HandymanProfiles.FindAsync(user.Id);
        Assert.That(profile!.IsActive, Is.False);
    }

    [Test]
    public async Task DeactivateAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _service.DeactivateAsync("nonexistent");

        Assert.That(success, Is.False);
    }

    // --- GetPendingVerificationAsync ---

    [Test]
    public async Task GetPendingVerificationAsync_ReturnsOnlyActiveAndUnverifiedProfiles()
    {
        var user1 = CreateUser("user-1", "Иван", "Чакащ");
        var user2 = CreateUser("user-2", "Петър", "Верифициран");
        var user3 = CreateUser("user-3", "Георги", "Неактивен");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.AddRange(
            new HandymanProfile { UserId = user1.Id, IsActive = true, IsVerified = false },
            new HandymanProfile { UserId = user2.Id, IsActive = true, IsVerified = true },
            new HandymanProfile { UserId = user3.Id, IsActive = false, IsVerified = false }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetPendingVerificationAsync();

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].UserId, Is.EqualTo(user1.Id));
    }

    [Test]
    public async Task GetPendingVerificationAsync_WhenNoPending_ReturnsEmptyList()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true, IsVerified = true });
        await _context.SaveChangesAsync();

        var result = await _service.GetPendingVerificationAsync();

        Assert.That(result, Is.Empty);
    }

    // --- VerifyAsync ---

    [Test]
    public async Task VerifyAsync_WhenApproved_SetsIsVerifiedTrueAndReturnsTrue()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true, IsVerified = false });
        await _context.SaveChangesAsync();

        var success = await _service.VerifyAsync(user.Id, true);

        Assert.That(success, Is.True);
        var profile = await _context.HandymanProfiles.FindAsync(user.Id);
        Assert.That(profile!.IsVerified, Is.True);
    }

    [Test]
    public async Task VerifyAsync_WhenRejected_SetsIsVerifiedFalseAndReturnsTrue()
    {
        var user = CreateUser("user-1");
        await _context.SaveChangesAsync();

        _context.HandymanProfiles.Add(new HandymanProfile { UserId = user.Id, IsActive = true, IsVerified = true });
        await _context.SaveChangesAsync();

        var success = await _service.VerifyAsync(user.Id, false);

        Assert.That(success, Is.True);
        var profile = await _context.HandymanProfiles.FindAsync(user.Id);
        Assert.That(profile!.IsVerified, Is.False);
    }

    [Test]
    public async Task VerifyAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _service.VerifyAsync("nonexistent", true);

        Assert.That(success, Is.False);
    }
}
