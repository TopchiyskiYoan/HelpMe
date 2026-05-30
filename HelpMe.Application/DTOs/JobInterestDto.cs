using System.ComponentModel.DataAnnotations;

namespace HelpMe.Application.DTOs;

public class JobInterestDto
{
    public int Id { get; set; }
    public int JobId { get; set; }
    public string HandymanId { get; set; } = string.Empty;
    public string HandymanName { get; set; } = string.Empty;
    public decimal ProposedPrice { get; set; }
    public string? Note { get; set; }
    public DateTime SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class SubmitInterestDto
{
    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal ProposedPrice { get; set; }

    [MaxLength(1000)]
    public string? Note { get; set; }
}
