using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class Job
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    public decimal? ApproximateBudget { get; set; }

    public JobStatus Status { get; set; } = JobStatus.Open;

    [MaxLength(450)]
    public string ClientId { get; set; } = string.Empty;

    public int SubCategoryId { get; set; }

    public int CityId { get; set; }

    [MaxLength(450)]
    public string? SelectedHandymanId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ApplicationUser Client { get; set; } = null!;
    public ServiceSubCategory SubCategory { get; set; } = null!;
    public City City { get; set; } = null!;
    public HandymanProfile? SelectedHandyman { get; set; }
    public ICollection<JobInterest> Interests { get; set; } = new List<JobInterest>();
}
