using System.ComponentModel.DataAnnotations;

namespace HelpMe.Domain.Entities;

public class HandymanSubCategory
{
    [MaxLength(450)]
    public string UserId { get; set; } = string.Empty;

    public int SubCategoryId { get; set; }

    public HandymanProfile HandymanProfile { get; set; } = null!;
    public ServiceSubCategory SubCategory { get; set; } = null!;
}
