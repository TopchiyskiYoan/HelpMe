using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class ReviewResponse
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    [Required]
    [MaxLength(2000)]
    public string Content { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Review Review { get; set; } = null!;
}
