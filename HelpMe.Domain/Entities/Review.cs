using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class Review
{
    public int Id { get; set; }

    public int JobId { get; set; }

    [MaxLength(450)]
    public string ClientId { get; set; } = string.Empty;

    [MaxLength(450)]
    public string HandymanId { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Job Job { get; set; } = null!;
    public ApplicationUser Client { get; set; } = null!;
    public HandymanProfile Handyman { get; set; } = null!;
    public ReviewResponse? Response { get; set; }
}
