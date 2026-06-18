using Zariya.Models;

namespace Zariya.ViewModels;

public sealed class HomePageViewModel
{
    public int RegisteredDonorsCount { get; set; }
    public int AvailableDonorsCount { get; set; }
    public int LivesHelpedCount { get; set; }
    public int RequestsFulfilledCount { get; set; }
    public Dictionary<string, int> BloodInventory { get; set; } = new();
    public List<City> Cities { get; set; } = new();
    public List<Request> EmergencyRequests { get; set; } = new();
    public List<string> BloodGroups { get; set; } = new();
}
