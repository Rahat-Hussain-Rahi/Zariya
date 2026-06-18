using Microsoft.EntityFrameworkCore;
using Zariya.Constants;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.Models;

namespace Zariya.Services;

public sealed class DonationTypeService : IDonationTypeService
{
    private readonly ZariyaDbContext _context;

    public DonationTypeService(ZariyaDbContext context)
    {
        _context = context;
    }

    public async Task EnsureDefaultDonationTypesAsync()
    {
        var defaultTypes = new[]
        {
            new { Name = ApplicationConstants.DonationTypes.WholeBlood, Description = "Standard whole blood donation" },
            new { Name = ApplicationConstants.DonationTypes.Platelets, Description = "Platelet donation" },
            new { Name = ApplicationConstants.DonationTypes.Plasma, Description = "Plasma donation" }
        };

        var typeNames = defaultTypes.Select(type => type.Name).ToList();
        var existingTypeNames = await _context.DonationTypes
            .Where(type => typeNames.Contains(type.TypeName))
            .Select(type => type.TypeName)
            .ToListAsync();

        foreach (var type in defaultTypes.Where(type => !existingTypeNames.Contains(type.Name)))
        {
            _context.DonationTypes.Add(new DonationType
            {
                TypeName = type.Name,
                Description = type.Description,
                IsActive = true,
                CreatedAt = DateTime.Now
            });
        }

        if (_context.ChangeTracker.HasChanges())
        {
            await _context.SaveChangesAsync();
        }
    }

    public async Task<int> GetDefaultDonationTypeIdAsync()
    {
        var donationType = await _context.DonationTypes
            .OrderByDescending(dt => dt.TypeName == ApplicationConstants.DonationTypes.WholeBlood)
            .ThenBy(dt => dt.TypeId)
            .FirstOrDefaultAsync(dt => dt.IsActive);

        if (donationType != null)
        {
            return donationType.TypeId;
        }

        donationType = new DonationType
        {
            TypeName = ApplicationConstants.DonationTypes.WholeBlood,
            Description = "Standard whole blood donation",
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.DonationTypes.Add(donationType);
        await _context.SaveChangesAsync();
        return donationType.TypeId;
    }
}
