using System.ComponentModel.DataAnnotations;

namespace HelpMe.Application.DTOs;

public class JobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal? ApproximateBudget { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public int SubCategoryId { get; set; }
    public string SubCategoryName { get; set; } = string.Empty;
    public int CityId { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string? SelectedHandymanId { get; set; }
    public string? SelectedHandymanName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateJobDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal? ApproximateBudget { get; set; }

    [Required]
    public int SubCategoryId { get; set; }

    [Required]
    public int CityId { get; set; }
}
