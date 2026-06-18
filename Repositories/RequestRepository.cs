using Microsoft.EntityFrameworkCore;
using Zariya.Constants;
using Zariya.Data;
using Zariya.Interfaces;
using Zariya.Models;

namespace Zariya.Repositories;

public sealed class RequestRepository : Repository<Request>, IRequestRepository
{
    public RequestRepository(ZariyaDbContext context) : base(context)
    {
    }

    public Task<Request?> GetDetailsAsync(int requestId)
    {
        return Context.Requests
            .Include(r => r.Patient)
            .ThenInclude(p => p.City)
            .Include(r => r.Donor)
            .FirstOrDefaultAsync(r => r.RequestId == requestId);
    }

    public Task<List<Request>> GetEmergencyHeadlinesAsync(int take)
    {
        return Context.Requests
            .Include(r => r.Patient)
            .ThenInclude(p => p.City)
            .Where(r => r.Status == ApplicationConstants.RequestStatus.Pending && (r.Urgency == "urgent" || r.Urgency == "critical"))
            .OrderByDescending(r => r.CreatedAt)
            .Take(take)
            .ToListAsync();
    }
}
