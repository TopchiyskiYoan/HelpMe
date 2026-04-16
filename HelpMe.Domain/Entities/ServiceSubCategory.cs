using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class ServiceSubCategory
{
    public int Id { get; set; }
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public ServiceCategory Category { get; set; } = null!;
}
