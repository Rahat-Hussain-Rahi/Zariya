using Zariya.Models;

namespace Zariya.Interfaces;

public interface IReferenceDataService
{
    Task<List<City>> GetActiveCitiesAsync();
    List<string> GetBloodGroups();
}
