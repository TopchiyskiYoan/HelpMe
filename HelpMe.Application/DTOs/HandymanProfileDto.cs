using System.ComponentModel.DataAnnotations;

namespace HelpMe.Application.DTOs;

public class HandymanProfileDto
{
    public string UserId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsActive { get; set; }
    public List<SubCategoryDto> SubCategories { get; set; } = new();
    public List<CityDto> Cities { get; set; } = new();
}

public class CreateHandymanProfileDto
{
    [MaxLength(1000)]
    public string? Bio { get; set; }

    [Range(0, 60)]
    public int YearsOfExperience { get; set; }

    [Required]
    [MinLength(1)]
    public List<int> SubCategoryIds { get; set; } = new();

    [Required]
    [MinLength(1)]
    public List<int> CityIds { get; set; } = new();
}

public class UpdateHandymanProfileDto
{
    [MaxLength(1000)]
    public string? Bio { get; set; }

    [Range(0, 60)]
    public int YearsOfExperience { get; set; }

    [Required]
    [MinLength(1)]
    public List<int> SubCategoryIds { get; set; } = new();

    [Required]
    [MinLength(1)]
    public List<int> CityIds { get; set; } = new();
}
