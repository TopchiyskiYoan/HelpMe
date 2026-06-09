using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class Notification
{
    public int Id { get; set; }

    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Message { get; set; } = string.Empty;

    public bool IsRead { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public NotificationType Type { get; set; }

    public ApplicationUser User { get; set; } = null!;
}
