namespace Zariya.DTOs;

public sealed class DonorSearchCriteriaDto
{
    public string? BloodGroup { get; set; }
    public int? CityId { get; set; }
    public string? CityName { get; set; }
    public bool AvailableNow { get; set; } = true;
    public bool VerifiedOnly { get; set; } = true;
    public string? SortBy { get; set; } = "Closest First";
}
