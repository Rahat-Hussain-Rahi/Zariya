using Microsoft.EntityFrameworkCore;
using Zariya.Constants;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.ViewModels;

namespace Zariya.Services;

public sealed class HomeService : IHomeService
{
    private readonly ZariyaDbContext _context;
    private readonly IReferenceDataService _referenceData;
    private readonly IRequestRepository _requests;

    public HomeService(ZariyaDbContext context, IReferenceDataService referenceData, IRequestRepository requests)
    {
        _context = context;
        _referenceData = referenceData;
        _requests = requests;
    }

    public async Task<HomePageViewModel> GetHomePageAsync()
    {
        var bloodGroups = _referenceData.GetBloodGroups();
        var dbInventory = await _context.Donors
            .Where(d => d.Availability == ApplicationConstants.Availability.Available)
            .GroupBy(d => d.BloodGroup)
            .Select(g => new { BloodGroup = g.Key, Count = g.Count() })
            .ToListAsync();

        var baseInventory = new Dictionary<string, int>
        {
            { "A+", 4209 }, { "A-", 941 }, { "B+", 2124 }, { "B-", 541 },
            { "O+", 5379 }, { "O-", 2298 }, { "AB+", 1249 }, { "AB-", 309 }
        };

        return new HomePageViewModel
        {
            Cities = await _referenceData.GetActiveCitiesAsync(),
            EmergencyRequests = await _requests.GetEmergencyHeadlinesAsync(3),
            BloodGroups = bloodGroups,
            RegisteredDonorsCount = 12442 + await _context.Donors.CountAsync(),
            AvailableDonorsCount = 3884 + await _context.Donors.CountAsync(d => d.Availability == ApplicationConstants.Availability.Available),
            LivesHelpedCount = 45209 + await _context.Donations.CountAsync(),
            RequestsFulfilledCount = 8733 + await _context.Requests.CountAsync(r => r.IsFulfilled),
            BloodInventory = bloodGroups.ToDictionary(
                bg => bg,
                bg => (baseInventory.TryGetValue(bg, out var baseCount) ? baseCount : 0)
                    + (dbInventory.FirstOrDefault(x => x.BloodGroup == bg)?.Count ?? 0))
        };
    }
}
