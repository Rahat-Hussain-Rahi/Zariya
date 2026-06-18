using Microsoft.EntityFrameworkCore;
using Zariya.Constants;
using Zariya.Data;
using Zariya.DTOs;
using Zariya.Interfaces;
using Zariya.Models;

namespace Zariya.Repositories;

public sealed class DonorRepository : Repository<Donor>, IDonorRepository
{
    public DonorRepository(ZariyaDbContext context) : base(context)
    {
    }

    public async Task<List<Donor>> SearchAsync(DonorSearchCriteriaDto criteria)
    {
        var query = Context.Donors
            .Include(d => d.City)
            .Include(d => d.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(criteria.BloodGroup))
        {
            query = query.Where(d => d.BloodGroup == criteria.BloodGroup);
        }

        if (criteria.CityId.HasValue)
        {
            query = query.Where(d => d.CityId == criteria.CityId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(criteria.CityName))
        {
            query = query.Where(d => d.City != null && d.City.CityName.Contains(criteria.CityName));
        }

        if (criteria.AvailableNow)
        {
            query = query.Where(d => d.Availability == ApplicationConstants.Availability.Available);
        }

        if (criteria.VerifiedOnly)
        {
            query = query.Where(d => d.IsAdminVerified);
        }

        query = criteria.SortBy == "Name"
            ? query.OrderBy(d => d.FullName)
            : query.OrderBy(d => d.Availability == ApplicationConstants.Availability.Available ? 0 : 1)
                .ThenByDescending(d => d.IsAdminVerified)
                .ThenBy(d => d.FullName);

        return await query.ToListAsync();
    }

    public Task<Donor?> GetVerifiedDetailsAsync(int donorId)
    {
        return Context.Donors
            .Include(d => d.User)
            .Include(d => d.City)
            .FirstOrDefaultAsync(d => d.DonorId == donorId && d.IsAdminVerified);
    }

    public Task<Donor?> GetByUserIdAsync(int userId)
    {
        return Context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
    }

}
