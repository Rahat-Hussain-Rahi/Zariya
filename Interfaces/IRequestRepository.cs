using Zariya.Models;

namespace Zariya.Interfaces;

public interface IRequestRepository : IRepository<Request>
{
    Task<Request?> GetDetailsAsync(int requestId);
    Task<List<Request>> GetEmergencyHeadlinesAsync(int take);
}
