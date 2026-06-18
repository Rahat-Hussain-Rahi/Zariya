using Zariya.DTOs;
using Zariya.Models;

namespace Zariya.Interfaces;

public interface IDonorRepository : IRepository<Donor>
{
    Task<List<Donor>> SearchAsync(DonorSearchCriteriaDto criteria);
    Task<Donor?> GetVerifiedDetailsAsync(int donorId);
    Task<Donor?> GetByUserIdAsync(int userId);
}
