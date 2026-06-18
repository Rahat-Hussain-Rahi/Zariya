using Zariya.DTOs;
using Zariya.Models;
using Zariya.ViewModels;

namespace Zariya.Interfaces;

public interface IDonorService
{
    Task<DonorSearchViewModel> SearchAsync(DonorSearchCriteriaDto criteria);
    Task<DonorDetailsViewModel?> GetDetailsAsync(int donorId);
    Task<Donor?> GetByUserIdAsync(int userId);
}
