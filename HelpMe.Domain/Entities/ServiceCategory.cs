using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class ServiceCategory
{
    public int Id { get; set; }
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ServiceSubCategory> SubCategories { get; set; } = new List<ServiceSubCategory>();
}
