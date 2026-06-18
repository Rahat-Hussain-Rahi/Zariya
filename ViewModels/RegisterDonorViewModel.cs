using Zariya.Models;

namespace Zariya.ViewModels;

public sealed class RegisterDonorViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Cnic { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);
    public string Gender { get; set; } = "Male";
    public int CityId { get; set; }
    public string BloodGroup { get; set; } = "A+";
    public string? Bio { get; set; }
    public List<City> Cities { get; set; } = new();
    public List<string> BloodGroups { get; set; } = new();
}
