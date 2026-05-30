using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IJobInterestService
{
    Task<JobInterestDto?> SubmitInterestAsync(string handymanUserId, int jobId, SubmitInterestDto dto);
    Task<List<JobInterestDto>> GetInterestsForJobAsync(int jobId);
    Task<List<JobInterestDto>> GetHandymanInterestsAsync(string handymanUserId);
}
