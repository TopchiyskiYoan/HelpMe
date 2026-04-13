namespace HelpMe.Domain.Entities;

public class ServiceCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<ServiceSubCategory> SubCategories { get; set; } = new List<ServiceSubCategory>();
}
