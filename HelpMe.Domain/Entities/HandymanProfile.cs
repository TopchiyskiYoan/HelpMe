using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class HandymanProfile
{
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Bio { get; set; }

    public int YearsOfExperience { get; set; }

    public bool IsActive { get; set; } = true;

    public ApplicationUser User { get; set; } = null!;
    public ICollection<HandymanSubCategory> SubCategories { get; set; } = new List<HandymanSubCategory>();
    public ICollection<HandymanCity> Cities { get; set; } = new List<HandymanCity>();
}
