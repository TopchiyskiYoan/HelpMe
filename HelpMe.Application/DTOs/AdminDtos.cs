namespace HelpMe.Application.DTOs;

public class AdminUserDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AdminUserDetailDto
{
    public string Id { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsBanned { get; set; }
    public DateTime CreatedAt { get; set; }
    // Handyman fields
    public double? AverageRating { get; set; }
    public int? ReviewCount { get; set; }
    public bool? IsVerified { get; set; }
    public int? YearsOfExperience { get; set; }
    public List<string> SubCategories { get; set; } = [];
    public List<string> Cities { get; set; } = [];
    // Client fields
    public int? TotalJobs { get; set; }
    public int? CompletedJobs { get; set; }
}

public class AdminStatsDto
{
    public int TotalUsers { get; set; }
    public int TotalClients { get; set; }
    public int TotalHandymen { get; set; }
    public int VerifiedHandymen { get; set; }
    public int PendingVerifications { get; set; }
    public int TotalJobs { get; set; }
    public int OpenJobs { get; set; }
    public int InProgressJobs { get; set; }
    public int CompletedJobs { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
}

public class AdminJobDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string SubCategoryName { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AdminReviewDto
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string HandymanName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
