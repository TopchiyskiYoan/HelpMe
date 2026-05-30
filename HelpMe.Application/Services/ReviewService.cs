using HelpMe.Application.DTOs;
using HelpMe.Application.Interfaces;
using HelpMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelpMe.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IApplicationDbContext _context;

    public ReviewService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ReviewDto?> CreateReviewAsync(string clientId, CreateReviewDto dto)
    {
        var job = await _context.Jobs
            .FirstOrDefaultAsync(j => j.Id == dto.JobId);

        if (job is null) return null;
        if (job.Status != JobStatus.Completed) return null;
        if (job.ClientId != clientId) return null;
        if (job.SelectedHandymanId is null) return null;

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(r => r.JobId == dto.JobId);
        if (alreadyReviewed) return null;

        var review = new Review
        {
            JobId = dto.JobId,
            ClientId = clientId,
            HandymanId = job.SelectedHandymanId,
            Rating = dto.Rating,
            Comment = dto.Comment.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.Reviews.AddAsync(review);
        await _context.SaveChangesAsync();

        await UpdateHandymanRatingAsync(job.SelectedHandymanId);

        return await GetReviewDtoAsync(review.Id);
    }

    public async Task<List<ReviewDto>> GetHandymanReviewsAsync(string handymanId, int page = 1, int pageSize = 10)
    {
        var reviews = await _context.Reviews
            .Where(r => r.HandymanId == handymanId)
            .Include(r => r.Client)
            .Include(r => r.Response)
            .OrderByDescending(r => r.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return reviews.Select(ToDto).ToList();
    }

    public async Task<bool> RespondToReviewAsync(string handymanId, int reviewId, string content)
    {
        var review = await _context.Reviews
            .Include(r => r.Response)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null) return false;
        if (review.HandymanId != handymanId) return false;
        if (review.Response is not null) return false;

        var response = new ReviewResponse
        {
            ReviewId = reviewId,
            Content = content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        await _context.ReviewResponses.AddAsync(response);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteReviewAsync(int reviewId)
    {
        var review = await _context.Reviews
            .Include(r => r.Response)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        if (review is null) return false;

        var handymanId = review.HandymanId;

        if (review.Response is not null)
            _context.ReviewResponses.Remove(review.Response);

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();

        await UpdateHandymanRatingAsync(handymanId);
        return true;
    }

    private async Task UpdateHandymanRatingAsync(string handymanId)
    {
        var handyman = await _context.HandymanProfiles
            .FirstOrDefaultAsync(h => h.UserId == handymanId);

        if (handyman is null) return;

        var reviews = await _context.Reviews
            .Where(r => r.HandymanId == handymanId)
            .ToListAsync();

        handyman.ReviewCount = reviews.Count;
        handyman.AverageRating = reviews.Count > 0
            ? Math.Round(reviews.Average(r => r.Rating), 1)
            : 0;

        await _context.SaveChangesAsync();
    }

    private async Task<ReviewDto?> GetReviewDtoAsync(int reviewId)
    {
        var review = await _context.Reviews
            .Include(r => r.Client)
            .Include(r => r.Response)
            .FirstOrDefaultAsync(r => r.Id == reviewId);

        return review is null ? null : ToDto(review);
    }

    private static ReviewDto ToDto(Review r) => new()
    {
        Id = r.Id,
        JobId = r.JobId,
        ClientId = r.ClientId,
        ClientName = r.Client is not null ? $"{r.Client.FirstName} {r.Client.LastName}" : string.Empty,
        HandymanId = r.HandymanId,
        Rating = r.Rating,
        Comment = r.Comment,
        CreatedAt = r.CreatedAt,
        Response = r.Response is null ? null : new ReviewResponseDto
        {
            Id = r.Response.Id,
            Content = r.Response.Content,
            CreatedAt = r.Response.CreatedAt
        }
    };
}
