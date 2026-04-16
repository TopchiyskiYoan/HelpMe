using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class City
{
    public int Id { get; set; }
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public int RegionId { get; set; }
    public Region Region { get; set; } = null!;
}
