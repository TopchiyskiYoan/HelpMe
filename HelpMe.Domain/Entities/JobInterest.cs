using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class JobInterest
{
    public int Id { get; set; }

    public int JobId { get; set; }

    [MaxLength(450)]
    public string HandymanId { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal ProposedPrice { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public JobInterestStatus Status { get; set; } = JobInterestStatus.Pending;

    public Job Job { get; set; } = null!;
    public HandymanProfile Handyman { get; set; } = null!;
}
