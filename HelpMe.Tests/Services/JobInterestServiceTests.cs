using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class JobInterestServiceTests
{
    private ApplicationDbContext _context = null!;
    private JobInterestService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new JobInterestService(_context);
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

    private ServiceSubCategory CreateSubCategory()
    {
        var sub = new ServiceSubCategory
        {
            Name = "Течове",
            Description = "Описание",
            IsActive = true,
            Category = new ServiceCategory { Name = "ВиК", IsActive = true }
        };
        _context.ServiceSubCategories.Add(sub);
        return sub;
    }

    private City CreateCity()
    {
        var city = new City { Name = "София", Region = new Region { Name = "София (град)" } };
        _context.Cities.Add(city);
        return city;
    }

    private HandymanProfile CreateHandyman(string userId)
    {
        var profile = new HandymanProfile { UserId = userId, IsActive = true, IsVerified = true };
        _context.HandymanProfiles.Add(profile);
        return profile;
    }

    private Job CreateOpenJob(string clientId, int subCategoryId, int cityId)
    {
        var job = new Job
        {
            Title = "Тестова поръчка",
            Description = "Описание",
            ClientId = clientId,
            SubCategoryId = subCategoryId,
            CityId = cityId,
            Status = JobStatus.Open,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Jobs.Add(job);
        return job;
    }

    // --- SubmitInterestAsync ---

    [Test]
    public async Task SubmitInterestAsync_WhenJobOpenAndNoDuplicate_CreatesInterest()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1", "Димитър", "Колев");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        var dto = new SubmitInterestDto { ProposedPrice = 200, Note = "Мога веднага." };

        var result = await _service.SubmitInterestAsync(handymanUser.Id, job.Id, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.ProposedPrice, Is.EqualTo(200));
        Assert.That(result.Note, Is.EqualTo("Мога веднага."));
        Assert.That(result.Status, Is.EqualTo("Pending"));
        Assert.That(result.HandymanId, Is.EqualTo(handymanUser.Id));

        var inDb = await _context.JobInterests.FirstOrDefaultAsync(i => i.JobId == job.Id);
        Assert.That(inDb, Is.Not.Null);
    }

    [Test]
    public async Task SubmitInterestAsync_WhenJobNotFound_ReturnsNull()
    {
        var handymanUser = CreateUser("handyman-1");
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        await _context.SaveChangesAsync();

        var dto = new SubmitInterestDto { ProposedPrice = 100 };

        var result = await _service.SubmitInterestAsync(handymanUser.Id, 9999, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SubmitInterestAsync_WhenJobNotOpen_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job = new Job
        {
            Title = "Затворена поръчка",
            Description = "Описание",
            ClientId = client.Id,
            SubCategoryId = sub.Id,
            CityId = city.Id,
            Status = JobStatus.AwaitingConfirmation,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Jobs.Add(job);
        await _context.SaveChangesAsync();

        var dto = new SubmitInterestDto { ProposedPrice = 100 };

        var result = await _service.SubmitInterestAsync(handymanUser.Id, job.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SubmitInterestAsync_WhenDuplicateInterest_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        _context.JobInterests.Add(new JobInterest
        {
            JobId = job.Id,
            HandymanId = handymanUser.Id,
            ProposedPrice = 100,
            SubmittedAt = DateTime.UtcNow,
            Status = JobInterestStatus.Pending
        });
        await _context.SaveChangesAsync();

        var dto = new SubmitInterestDto { ProposedPrice = 200 };

        var result = await _service.SubmitInterestAsync(handymanUser.Id, job.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task SubmitInterestAsync_TrimsNote()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        var dto = new SubmitInterestDto { ProposedPrice = 100, Note = "  Свободен съм  " };

        var result = await _service.SubmitInterestAsync(handymanUser.Id, job.Id, dto);

        Assert.That(result!.Note, Is.EqualTo("Свободен съм"));
    }

    // --- GetInterestsForJobAsync ---

    [Test]
    public async Task GetInterestsForJobAsync_ReturnsAllInterestsForJob()
    {
        var client = CreateUser("client-1");
        var handyman1 = CreateUser("handyman-1", "Иван", "Колев");
        var handyman2 = CreateUser("handyman-2", "Петър", "Иванов");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handyman1.Id);
        CreateHandyman(handyman2.Id);
        var job = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        _context.JobInterests.AddRange(
            new JobInterest { JobId = job.Id, HandymanId = handyman1.Id, ProposedPrice = 100, SubmittedAt = DateTime.UtcNow, Status = JobInterestStatus.Pending },
            new JobInterest { JobId = job.Id, HandymanId = handyman2.Id, ProposedPrice = 150, SubmittedAt = DateTime.UtcNow, Status = JobInterestStatus.Pending }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetInterestsForJobAsync(job.Id);

        Assert.That(result, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetInterestsForJobAsync_WhenNoInterests_ReturnsEmptyList()
    {
        var result = await _service.GetInterestsForJobAsync(9999);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetInterestsForJobAsync_IncludesHandymanName()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1", "Димитър", "Колев");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        _context.JobInterests.Add(new JobInterest
        {
            JobId = job.Id,
            HandymanId = handymanUser.Id,
            ProposedPrice = 100,
            SubmittedAt = DateTime.UtcNow,
            Status = JobInterestStatus.Pending
        });
        await _context.SaveChangesAsync();

        var result = await _service.GetInterestsForJobAsync(job.Id);

        Assert.That(result[0].HandymanName, Is.EqualTo("Димитър Колев"));
    }

    // --- GetHandymanInterestsAsync ---

    [Test]
    public async Task GetHandymanInterestsAsync_ReturnsOnlyHandymanInterests()
    {
        var client = CreateUser("client-1");
        var handyman1 = CreateUser("handyman-1");
        var handyman2 = CreateUser("handyman-2");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handyman1.Id);
        CreateHandyman(handyman2.Id);
        var job1 = CreateOpenJob(client.Id, sub.Id, city.Id);
        var job2 = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        _context.JobInterests.AddRange(
            new JobInterest { JobId = job1.Id, HandymanId = handyman1.Id, ProposedPrice = 100, SubmittedAt = DateTime.UtcNow, Status = JobInterestStatus.Pending },
            new JobInterest { JobId = job2.Id, HandymanId = handyman1.Id, ProposedPrice = 200, SubmittedAt = DateTime.UtcNow, Status = JobInterestStatus.Pending },
            new JobInterest { JobId = job1.Id, HandymanId = handyman2.Id, ProposedPrice = 90, SubmittedAt = DateTime.UtcNow, Status = JobInterestStatus.Pending }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetHandymanInterestsAsync(handyman1.Id);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(i => i.HandymanId == handyman1.Id), Is.True);
    }

    [Test]
    public async Task GetHandymanInterestsAsync_WhenNoInterests_ReturnsEmptyList()
    {
        var result = await _service.GetHandymanInterestsAsync("nonexistent");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetHandymanInterestsAsync_ReturnsNewestFirst()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandyman(handymanUser.Id);
        var job1 = CreateOpenJob(client.Id, sub.Id, city.Id);
        var job2 = CreateOpenJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        _context.JobInterests.AddRange(
            new JobInterest { JobId = job1.Id, HandymanId = handymanUser.Id, ProposedPrice = 100, SubmittedAt = DateTime.UtcNow.AddHours(-2), Status = JobInterestStatus.Pending },
            new JobInterest { JobId = job2.Id, HandymanId = handymanUser.Id, ProposedPrice = 200, SubmittedAt = DateTime.UtcNow.AddHours(-1), Status = JobInterestStatus.Pending }
        );
        await _context.SaveChangesAsync();

        var result = await _service.GetHandymanInterestsAsync(handymanUser.Id);

        Assert.That(result[0].ProposedPrice, Is.EqualTo(200));
        Assert.That(result[1].ProposedPrice, Is.EqualTo(100));
    }
}
