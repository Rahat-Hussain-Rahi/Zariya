using Zariya.Models;

namespace Zariya.ViewModels;

public sealed class DonorSearchViewModel
{
    public List<Donor> Donors { get; set; } = new();
    public List<City> Cities { get; set; } = new();
    public List<string> BloodGroups { get; set; } = new();
}
