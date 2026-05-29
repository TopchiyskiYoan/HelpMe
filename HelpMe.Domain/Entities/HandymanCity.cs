using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class HandymanCity
{
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    public int CityId { get; set; }

    public HandymanProfile HandymanProfile { get; set; } = null!;
    public City City { get; set; } = null!;
}
