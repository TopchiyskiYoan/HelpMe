using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class JobServiceTests
{
    private ApplicationDbContext _context = null!;
    private JobService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new JobService(_context);
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

    private HandymanProfile CreateHandymanProfile(string userId, int subCategoryId, int cityId, bool isVerified = true)
    {
        var profile = new HandymanProfile
        {
            UserId = userId,
            IsActive = true,
            IsVerified = isVerified,
            SubCategories = new List<HandymanSubCategory>
            {
                new HandymanSubCategory { UserId = userId, SubCategoryId = subCategoryId }
            },
            Cities = new List<HandymanCity>
            {
                new HandymanCity { UserId = userId, CityId = cityId }
            }
        };
        _context.HandymanProfiles.Add(profile);
        return profile;
    }

    private Job CreateJob(string clientId, int subCategoryId, int cityId, JobStatus status = JobStatus.Open)
    {
        var job = new Job
        {
            Title = "Тестова поръчка",
            Description = "Тестово описание",
            ApproximateBudget = 100,
            ClientId = clientId,
            SubCategoryId = subCategoryId,
            CityId = cityId,
            Status = status,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Jobs.Add(job);
        return job;
    }

    // --- CreateAsync ---

    [Test]
    public async Task CreateAsync_SavesJobAndReturnsDto()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var dto = new CreateJobDto
        {
            Title = "Смяна на кран",
            Description = "Нуждая се от помощ",
            ApproximateBudget = 150,
            SubCategoryId = sub.Id,
            CityId = city.Id
        };

        var result = await _service.CreateAsync(client.Id, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Title, Is.EqualTo("Смяна на кран"));
        Assert.That(result.Status, Is.EqualTo("Open"));
        Assert.That(result.ClientId, Is.EqualTo(client.Id));

        var inDb = await _context.Jobs.FindAsync(result.Id);
        Assert.That(inDb, Is.Not.Null);
    }

    [Test]
    public async Task CreateAsync_TrimsTitle()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var dto = new CreateJobDto
        {
            Title = "  Смяна на кран  ",
            Description = "  Описание  ",
            SubCategoryId = sub.Id,
            CityId = city.Id
        };

        var result = await _service.CreateAsync(client.Id, dto);

        Assert.That(result.Title, Is.EqualTo("Смяна на кран"));
    }

    // --- GetByIdAsync ---

    [Test]
    public async Task GetByIdAsync_WhenExists_ReturnsJobDto()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        var result = await _service.GetByIdAsync(job.Id);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Id, Is.EqualTo(job.Id));
        Assert.That(result.Status, Is.EqualTo("Open"));
    }

    [Test]
    public async Task GetByIdAsync_WhenNotFound_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(9999);

        Assert.That(result, Is.Null);
    }

    // --- GetClientJobsAsync ---

    [Test]
    public async Task GetClientJobsAsync_ReturnsOnlyClientJobs()
    {
        var client1 = CreateUser("client-1");
        var client2 = CreateUser("client-2");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateJob(client1.Id, sub.Id, city.Id);
        CreateJob(client1.Id, sub.Id, city.Id);
        CreateJob(client2.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        var result = await _service.GetClientJobsAsync(client1.Id);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(j => j.ClientId == client1.Id), Is.True);
    }

    [Test]
    public async Task GetClientJobsAsync_WhenNoJobs_ReturnsEmptyList()
    {
        var result = await _service.GetClientJobsAsync("nonexistent-client");

        Assert.That(result, Is.Empty);
    }

    // --- GetOpenJobsForHandymanAsync ---

    [Test]
    public async Task GetOpenJobsForHandymanAsync_ReturnsMatchingOpenJobs()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory("Течове");
        var otherSub = CreateSubCategory("Бояджийство");
        var city = CreateCity("София");
        var otherCity = CreateCity("Варна");
        await _context.SaveChangesAsync();

        CreateHandymanProfile(handymanUser.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        CreateJob(client.Id, sub.Id, city.Id);           // matches
        CreateJob(client.Id, sub.Id, city.Id);           // matches
        CreateJob(client.Id, otherSub.Id, city.Id);      // wrong category
        CreateJob(client.Id, sub.Id, otherCity.Id);      // wrong city
        var cancelledJob = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Cancelled); // wrong status
        await _context.SaveChangesAsync();

        var result = await _service.GetOpenJobsForHandymanAsync(handymanUser.Id);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(j => j.Status == "Open"), Is.True);
    }

    [Test]
    public async Task GetOpenJobsForHandymanAsync_WhenHandymanNotVerified_ReturnsEmpty()
    {
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        CreateHandymanProfile(handymanUser.Id, sub.Id, city.Id, isVerified: false);
        await _context.SaveChangesAsync();

        var result = await _service.GetOpenJobsForHandymanAsync(handymanUser.Id);

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetOpenJobsForHandymanAsync_WhenHandymanNotFound_ReturnsEmpty()
    {
        var result = await _service.GetOpenJobsForHandymanAsync("nonexistent");

        Assert.That(result, Is.Empty);
    }

    // --- CancelAsync ---

    [Test]
    public async Task CancelAsync_WhenOpenAndOwner_CancelsAndReturnsTrue()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Open);
        await _context.SaveChangesAsync();

        var success = await _service.CancelAsync(job.Id, client.Id);

        Assert.That(success, Is.True);
        var updated = await _context.Jobs.FindAsync(job.Id);
        Assert.That(updated!.Status, Is.EqualTo(JobStatus.Cancelled));
    }

    [Test]
    public async Task CancelAsync_WhenAwaitingConfirmationAndOwner_CancelsAndReturnsTrue()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        await _context.SaveChangesAsync();

        var success = await _service.CancelAsync(job.Id, client.Id);

        Assert.That(success, Is.True);
    }

    [Test]
    public async Task CancelAsync_WhenNotOwner_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var other = CreateUser("other-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id);
        await _context.SaveChangesAsync();

        var success = await _service.CancelAsync(job.Id, other.Id);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task CancelAsync_WhenInProgress_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.InProgress);
        await _context.SaveChangesAsync();

        var success = await _service.CancelAsync(job.Id, client.Id);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task CancelAsync_WhenJobNotFound_ReturnsFalse()
    {
        var success = await _service.CancelAsync(9999, "any-client");

        Assert.That(success, Is.False);
    }

    // --- SelectHandymanAsync ---

    [Test]
    public async Task SelectHandymanAsync_WhenValid_SelectsHandymanAndChangesStatus()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Open);
        var interest = new JobInterest
        {
            HandymanId = handymanUser.Id,
            ProposedPrice = 200,
            Status = JobInterestStatus.Pending
        };
        job.Interests.Add(interest);
        await _context.SaveChangesAsync();

        var success = await _service.SelectHandymanAsync(job.Id, interest.Id, client.Id);

        Assert.That(success, Is.True);
        var updatedJob = await _context.Jobs.Include(j => j.Interests).FirstAsync(j => j.Id == job.Id);
        Assert.That(updatedJob.Status, Is.EqualTo(JobStatus.AwaitingConfirmation));
        Assert.That(updatedJob.SelectedHandymanId, Is.EqualTo(handymanUser.Id));
        Assert.That(updatedJob.Interests.First(i => i.Id == interest.Id).Status, Is.EqualTo(JobInterestStatus.Selected));
    }

    [Test]
    public async Task SelectHandymanAsync_WithMultipleInterests_RejectsOthers()
    {
        var client = CreateUser("client-1");
        var handyman1 = CreateUser("handyman-1");
        var handyman2 = CreateUser("handyman-2");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Open);
        var interest1 = new JobInterest { HandymanId = handyman1.Id, ProposedPrice = 200, Status = JobInterestStatus.Pending };
        var interest2 = new JobInterest { HandymanId = handyman2.Id, ProposedPrice = 150, Status = JobInterestStatus.Pending };
        job.Interests.Add(interest1);
        job.Interests.Add(interest2);
        await _context.SaveChangesAsync();

        await _service.SelectHandymanAsync(job.Id, interest1.Id, client.Id);

        var updatedJob = await _context.Jobs.Include(j => j.Interests).FirstAsync(j => j.Id == job.Id);
        var i1 = updatedJob.Interests.First(i => i.Id == interest1.Id);
        var i2 = updatedJob.Interests.First(i => i.Id == interest2.Id);
        Assert.That(i1.Status, Is.EqualTo(JobInterestStatus.Selected));
        Assert.That(i2.Status, Is.EqualTo(JobInterestStatus.Rejected));
    }

    [Test]
    public async Task SelectHandymanAsync_WhenNotOwner_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var other = CreateUser("other-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id);
        var interest = new JobInterest { HandymanId = other.Id, ProposedPrice = 100, Status = JobInterestStatus.Pending };
        job.Interests.Add(interest);
        await _context.SaveChangesAsync();

        var success = await _service.SelectHandymanAsync(job.Id, interest.Id, other.Id);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task SelectHandymanAsync_WhenJobNotOpen_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        var interest = new JobInterest { HandymanId = handymanUser.Id, ProposedPrice = 100, Status = JobInterestStatus.Pending };
        job.Interests.Add(interest);
        await _context.SaveChangesAsync();

        var success = await _service.SelectHandymanAsync(job.Id, interest.Id, client.Id);

        Assert.That(success, Is.False);
    }

    // --- ConfirmJobAsync ---

    [Test]
    public async Task ConfirmJobAsync_WhenSelectedHandyman_ChangesStatusToInProgress()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        job.SelectedHandymanId = handymanUser.Id;
        await _context.SaveChangesAsync();

        var success = await _service.ConfirmJobAsync(job.Id, handymanUser.Id);

        Assert.That(success, Is.True);
        var updated = await _context.Jobs.FindAsync(job.Id);
        Assert.That(updated!.Status, Is.EqualTo(JobStatus.InProgress));
    }

    [Test]
    public async Task ConfirmJobAsync_WhenWrongHandyman_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var selectedHandyman = CreateUser("handyman-1");
        var otherHandyman = CreateUser("handyman-2");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        job.SelectedHandymanId = selectedHandyman.Id;
        await _context.SaveChangesAsync();

        var success = await _service.ConfirmJobAsync(job.Id, otherHandyman.Id);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task ConfirmJobAsync_WhenStatusNotAwaitingConfirmation_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Open);
        job.SelectedHandymanId = handymanUser.Id;
        await _context.SaveChangesAsync();

        var success = await _service.ConfirmJobAsync(job.Id, handymanUser.Id);

        Assert.That(success, Is.False);
    }

    // --- DeclineJobAsync ---

    [Test]
    public async Task DeclineJobAsync_WhenSelectedHandyman_ResetsJobToOpen()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        job.SelectedHandymanId = handymanUser.Id;
        var interest = new JobInterest
        {
            HandymanId = handymanUser.Id,
            ProposedPrice = 100,
            Status = JobInterestStatus.Selected
        };
        job.Interests.Add(interest);
        await _context.SaveChangesAsync();

        var success = await _service.DeclineJobAsync(job.Id, handymanUser.Id);

        Assert.That(success, Is.True);
        var updated = await _context.Jobs.Include(j => j.Interests).FirstAsync(j => j.Id == job.Id);
        Assert.That(updated.Status, Is.EqualTo(JobStatus.Open));
        Assert.That(updated.SelectedHandymanId, Is.Null);
        Assert.That(updated.Interests.First().Status, Is.EqualTo(JobInterestStatus.Pending));
    }

    [Test]
    public async Task DeclineJobAsync_WhenWrongHandyman_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var selectedHandyman = CreateUser("handyman-1");
        var otherHandyman = CreateUser("handyman-2");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.AwaitingConfirmation);
        job.SelectedHandymanId = selectedHandyman.Id;
        await _context.SaveChangesAsync();

        var success = await _service.DeclineJobAsync(job.Id, otherHandyman.Id);

        Assert.That(success, Is.False);
    }

    // --- CompleteJobAsync ---

    [Test]
    public async Task CompleteJobAsync_WhenClientAndInProgress_Completes()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.InProgress);
        job.SelectedHandymanId = handymanUser.Id;
        await _context.SaveChangesAsync();

        var success = await _service.CompleteJobAsync(job.Id, client.Id);

        Assert.That(success, Is.True);
        var updated = await _context.Jobs.FindAsync(job.Id);
        Assert.That(updated!.Status, Is.EqualTo(JobStatus.Completed));
    }

    [Test]
    public async Task CompleteJobAsync_WhenHandymanAndInProgress_Completes()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.InProgress);
        job.SelectedHandymanId = handymanUser.Id;
        await _context.SaveChangesAsync();

        var success = await _service.CompleteJobAsync(job.Id, handymanUser.Id);

        Assert.That(success, Is.True);
    }

    [Test]
    public async Task CompleteJobAsync_WhenNotInProgress_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.Open);
        await _context.SaveChangesAsync();

        var success = await _service.CompleteJobAsync(job.Id, client.Id);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task CompleteJobAsync_WhenUnauthorized_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var stranger = CreateUser("stranger-1");
        var sub = CreateSubCategory();
        var city = CreateCity();
        await _context.SaveChangesAsync();

        var job = CreateJob(client.Id, sub.Id, city.Id, JobStatus.InProgress);
        job.SelectedHandymanId = handymanUser.Id;
        await _context.SaveChangesAsync();

        var success = await _service.CompleteJobAsync(job.Id, stranger.Id);

        Assert.That(success, Is.False);
    }
}
