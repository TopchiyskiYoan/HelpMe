using HelpMe.Application.DTOs;

namespace HelpMe.Application.Interfaces;

public interface IJobService
{
    Task<JobDto> CreateAsync(string clientId, CreateJobDto dto);
    Task<JobDto?> GetByIdAsync(int id);
    Task<List<JobDto>> GetClientJobsAsync(string clientId);
    Task<List<JobDto>> GetOpenJobsForHandymanAsync(string handymanUserId);
    Task<bool> CancelAsync(int id, string requesterId);
    Task<bool> SelectHandymanAsync(int jobId, int interestId, string clientId);
    Task<bool> ConfirmJobAsync(int jobId, string handymanUserId);
    Task<bool> DeclineJobAsync(int jobId, string handymanUserId);
    Task<bool> CompleteJobAsync(int jobId, string requesterId);
}
