namespace HelpMe.Domain.Entities;

public class ServiceSubCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int CategoryId { get; set; }
    public ServiceCategory Category { get; set; } = null!;
}
