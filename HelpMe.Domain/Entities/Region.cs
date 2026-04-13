namespace HelpMe.Domain.Entities;

public class Region
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<City> Cities { get; set; } = new List<City>();
}
