using Microsoft.EntityFrameworkCore;
using Zariya.Constants;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.Models;

namespace Zariya.Services;

public sealed class ReferenceDataService : IReferenceDataService
{
    private readonly ZariyaDbContext _context;

    public ReferenceDataService(ZariyaDbContext context)
    {
        _context = context;
    }

    public Task<List<City>> GetActiveCitiesAsync()
    {
        return _context.Cities
            .Where(c => c.IsActive)
            .OrderBy(c => c.CityName)
            .ToListAsync();
    }

    public List<string> GetBloodGroups() => ApplicationConstants.BloodGroups.ToList();
}
