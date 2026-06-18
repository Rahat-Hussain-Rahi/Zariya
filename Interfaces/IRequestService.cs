using Zariya.ViewModels;

namespace Zariya.Interfaces;

public interface IRequestService
{
    Task<RequestDetailsViewModel?> GetDetailsAsync(int requestId);
}
