using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IReviewService
{
    Task<ReviewDto?> CreateReviewAsync(string clientId, CreateReviewDto dto);
    Task<List<ReviewDto>> GetHandymanReviewsAsync(string handymanId, int page = 1, int pageSize = 10);
    Task<bool> RespondToReviewAsync(string handymanId, int reviewId, string content);
    Task<bool> DeleteReviewAsync(int reviewId);
}
