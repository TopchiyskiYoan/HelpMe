using HelpMe.Application.DTOs;
using HelpMe.Application.Services;
using HelpMe.Domain.Entities;
using HelpMe.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Tests.Services;

[TestFixture]
public class ReviewServiceTests
{
    private ApplicationDbContext _context = null!;
    private ReviewService _service = null!;

    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _service = new ReviewService(_context);
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

    private HandymanProfile CreateHandymanProfile(string userId)
    {
        var profile = new HandymanProfile
        {
            UserId = userId,
            IsActive = true,
            IsVerified = true
        };
        _context.HandymanProfiles.Add(profile);
        return profile;
    }

    private Job CreateJob(string clientId, string? handymanId, JobStatus status = JobStatus.Completed)
    {
        var sub = new ServiceSubCategory
        {
            Name = "Течове",
            Description = "Описание",
            IsActive = true,
            Category = new ServiceCategory { Name = "ВиК", IsActive = true }
        };
        _context.ServiceSubCategories.Add(sub);

        var city = new City
        {
            Name = "София",
            Region = new Region { Name = "Sofia" }
        };
        _context.Cities.Add(city);

        var job = new Job
        {
            Title = "Тестова поръчка",
            Description = "Описание",
            ClientId = clientId,
            SubCategoryId = sub.Id,
            CityId = city.Id,
            SelectedHandymanId = handymanId,
            Status = status,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            UpdatedAt = DateTime.UtcNow.AddDays(-1)
        };
        _context.Jobs.Add(job);
        return job;
    }

    // --- CreateReviewAsync ---

    [Test]
    public async Task CreateReviewAsync_WhenValid_CreatesReviewAndReturnsDto()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto
        {
            JobId = job.Id,
            Rating = 5,
            Comment = "Страхотна работа!"
        };

        var result = await _service.CreateReviewAsync(client.Id, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Rating, Is.EqualTo(5));
        Assert.That(result.Comment, Is.EqualTo("Страхотна работа!"));
        Assert.That(result.ClientId, Is.EqualTo(client.Id));
        Assert.That(result.HandymanId, Is.EqualTo(handymanUser.Id));

        var inDb = await _context.Reviews.FindAsync(result.Id);
        Assert.That(inDb, Is.Not.Null);
    }

    [Test]
    public async Task CreateReviewAsync_TrimsComment()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto
        {
            JobId = job.Id,
            Rating = 4,
            Comment = "  Добра работа!  "
        };

        var result = await _service.CreateReviewAsync(client.Id, dto);

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.Comment, Is.EqualTo("Добра работа!"));
    }

    [Test]
    public async Task CreateReviewAsync_WhenJobNotCompleted_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.InProgress);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto { JobId = job.Id, Rating = 5, Comment = "OK" };

        var result = await _service.CreateReviewAsync(client.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateReviewAsync_WhenClientNotOwner_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var otherUser = CreateUser("other-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto { JobId = job.Id, Rating = 5, Comment = "OK" };

        var result = await _service.CreateReviewAsync(otherUser.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateReviewAsync_WhenAlreadyReviewed_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var existing = new Review
        {
            JobId = job.Id,
            ClientId = client.Id,
            HandymanId = handymanUser.Id,
            Rating = 4,
            Comment = "Вече оставен."
        };
        _context.Reviews.Add(existing);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto { JobId = job.Id, Rating = 5, Comment = "Дубликат" };

        var result = await _service.CreateReviewAsync(client.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateReviewAsync_WhenJobNotFound_ReturnsNull()
    {
        var result = await _service.CreateReviewAsync("client-1", new CreateReviewDto { JobId = 9999, Rating = 5, Comment = "OK" });

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateReviewAsync_WhenNoSelectedHandyman_ReturnsNull()
    {
        var client = CreateUser("client-1");
        var job = CreateJob(client.Id, null, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto { JobId = job.Id, Rating = 5, Comment = "OK" };

        var result = await _service.CreateReviewAsync(client.Id, dto);

        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task CreateReviewAsync_UpdatesHandymanRating()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var profile = CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id, JobStatus.Completed);
        await _context.SaveChangesAsync();

        var dto = new CreateReviewDto { JobId = job.Id, Rating = 4, Comment = "Добро" };
        await _service.CreateReviewAsync(client.Id, dto);

        var updatedProfile = await _context.HandymanProfiles.FindAsync(handymanUser.Id);
        Assert.That(updatedProfile!.ReviewCount, Is.EqualTo(1));
        Assert.That(updatedProfile.AverageRating, Is.EqualTo(4.0));
    }

    // --- GetHandymanReviewsAsync ---

    [Test]
    public async Task GetHandymanReviewsAsync_ReturnsReviewsForHandyman()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job1 = CreateJob(client.Id, handymanUser.Id);
        var job2 = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        _context.Reviews.Add(new Review { JobId = job1.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "A" });
        _context.Reviews.Add(new Review { JobId = job2.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 3, Comment = "B" });
        await _context.SaveChangesAsync();

        var result = await _service.GetHandymanReviewsAsync(handymanUser.Id);

        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(r => r.HandymanId == handymanUser.Id), Is.True);
    }

    [Test]
    public async Task GetHandymanReviewsAsync_WhenNoReviews_ReturnsEmptyList()
    {
        var result = await _service.GetHandymanReviewsAsync("nonexistent");

        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task GetHandymanReviewsAsync_IsPagedCorrectly()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        await _context.SaveChangesAsync();

        for (int i = 0; i < 5; i++)
        {
            var job = CreateJob(client.Id, handymanUser.Id);
            await _context.SaveChangesAsync();
            _context.Reviews.Add(new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = $"Review {i}" });
        }
        await _context.SaveChangesAsync();

        var page1 = await _service.GetHandymanReviewsAsync(handymanUser.Id, page: 1, pageSize: 3);
        var page2 = await _service.GetHandymanReviewsAsync(handymanUser.Id, page: 2, pageSize: 3);

        Assert.That(page1, Has.Count.EqualTo(3));
        Assert.That(page2, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task GetHandymanReviewsAsync_IncludesResponse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "Super" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        _context.ReviewResponses.Add(new ReviewResponse { ReviewId = review.Id, Content = "Благодаря!" });
        await _context.SaveChangesAsync();

        var result = await _service.GetHandymanReviewsAsync(handymanUser.Id);

        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].Response, Is.Not.Null);
        Assert.That(result[0].Response!.Content, Is.EqualTo("Благодаря!"));
    }

    // --- RespondToReviewAsync ---

    [Test]
    public async Task RespondToReviewAsync_WhenValid_CreatesResponseAndReturnsTrue()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "Super" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var success = await _service.RespondToReviewAsync(handymanUser.Id, review.Id, "Благодаря много!");

        Assert.That(success, Is.True);
        var response = await _context.ReviewResponses.FirstOrDefaultAsync(r => r.ReviewId == review.Id);
        Assert.That(response, Is.Not.Null);
        Assert.That(response!.Content, Is.EqualTo("Благодаря много!"));
    }

    [Test]
    public async Task RespondToReviewAsync_TrimsContent()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "Super" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await _service.RespondToReviewAsync(handymanUser.Id, review.Id, "  Благодаря!  ");

        var response = await _context.ReviewResponses.FirstOrDefaultAsync(r => r.ReviewId == review.Id);
        Assert.That(response!.Content, Is.EqualTo("Благодаря!"));
    }

    [Test]
    public async Task RespondToReviewAsync_WhenWrongHandyman_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var otherHandyman = CreateUser("handyman-2");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 3, Comment = "OK" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var success = await _service.RespondToReviewAsync(otherHandyman.Id, review.Id, "Грешен майстор");

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task RespondToReviewAsync_WhenAlreadyResponded_ReturnsFalse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "Super" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        _context.ReviewResponses.Add(new ReviewResponse { ReviewId = review.Id, Content = "Вече отговорено." });
        await _context.SaveChangesAsync();

        var success = await _service.RespondToReviewAsync(handymanUser.Id, review.Id, "Втори отговор");

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task RespondToReviewAsync_WhenReviewNotFound_ReturnsFalse()
    {
        var success = await _service.RespondToReviewAsync("handyman-1", 9999, "Отговор");

        Assert.That(success, Is.False);
    }

    // --- DeleteReviewAsync ---

    [Test]
    public async Task DeleteReviewAsync_WhenExists_DeletesAndReturnsTrue()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 1, Comment = "Лошо" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var success = await _service.DeleteReviewAsync(review.Id);

        Assert.That(success, Is.True);
        var deleted = await _context.Reviews.FindAsync(review.Id);
        Assert.That(deleted, Is.Null);
    }

    [Test]
    public async Task DeleteReviewAsync_AlsoDeletesResponse()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        CreateHandymanProfile(handymanUser.Id);
        var job = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review = new Review { JobId = job.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 2, Comment = "Не страхотно" };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var response = new ReviewResponse { ReviewId = review.Id, Content = "Извиняваме се." };
        _context.ReviewResponses.Add(response);
        await _context.SaveChangesAsync();

        await _service.DeleteReviewAsync(review.Id);

        var deletedResponse = await _context.ReviewResponses.FindAsync(response.Id);
        Assert.That(deletedResponse, Is.Null);
    }

    [Test]
    public async Task DeleteReviewAsync_WhenNotFound_ReturnsFalse()
    {
        var success = await _service.DeleteReviewAsync(9999);

        Assert.That(success, Is.False);
    }

    [Test]
    public async Task DeleteReviewAsync_UpdatesHandymanRating()
    {
        var client = CreateUser("client-1");
        var handymanUser = CreateUser("handyman-1");
        var profile = CreateHandymanProfile(handymanUser.Id);
        var job1 = CreateJob(client.Id, handymanUser.Id);
        var job2 = CreateJob(client.Id, handymanUser.Id);
        await _context.SaveChangesAsync();

        var review1 = new Review { JobId = job1.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 5, Comment = "A" };
        var review2 = new Review { JobId = job2.Id, ClientId = client.Id, HandymanId = handymanUser.Id, Rating = 3, Comment = "B" };
        _context.Reviews.AddRange(review1, review2);
        profile.ReviewCount = 2;
        profile.AverageRating = 4.0;
        await _context.SaveChangesAsync();

        await _service.DeleteReviewAsync(review2.Id);

        var updated = await _context.HandymanProfiles.FindAsync(handymanUser.Id);
        Assert.That(updated!.ReviewCount, Is.EqualTo(1));
        Assert.That(updated.AverageRating, Is.EqualTo(5.0));
    }
}
