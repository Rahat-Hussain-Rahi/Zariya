using Zariya.Interfaces;
using Zariya.ViewModels;

namespace Zariya.Services;

public sealed class RequestService : IRequestService
{
    private readonly IRequestRepository _requests;

    public RequestService(IRequestRepository requests)
    {
        _requests = requests;
    }

    public async Task<RequestDetailsViewModel?> GetDetailsAsync(int requestId)
    {
        var request = await _requests.GetDetailsAsync(requestId);
        return request == null ? null : new RequestDetailsViewModel { BloodRequest = request };
    }
}
