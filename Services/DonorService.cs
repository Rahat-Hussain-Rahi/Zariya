using Zariya.DTOs;
using Zariya.Interfaces;
using Zariya.Models;
using Zariya.ViewModels;

namespace Zariya.Services;

public sealed class DonorService : IDonorService
{
    private readonly IDonorRepository _donors;
    private readonly IReferenceDataService _referenceData;

    public DonorService(IDonorRepository donors, IReferenceDataService referenceData)
    {
        _donors = donors;
        _referenceData = referenceData;
    }

    public async Task<DonorSearchViewModel> SearchAsync(DonorSearchCriteriaDto criteria)
    {
        return new DonorSearchViewModel
        {
            Donors = await _donors.SearchAsync(criteria),
            Cities = await _referenceData.GetActiveCitiesAsync(),
            BloodGroups = _referenceData.GetBloodGroups()
        };
    }

    public async Task<DonorDetailsViewModel?> GetDetailsAsync(int donorId)
    {
        var donor = await _donors.GetVerifiedDetailsAsync(donorId);
        if (donor == null)
        {
            return null;
        }

        return new DonorDetailsViewModel
        {
            Donor = donor
        };
    }

    public Task<Donor?> GetByUserIdAsync(int userId) => _donors.GetByUserIdAsync(userId);
}
