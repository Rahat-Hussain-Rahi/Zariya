using Zariya.DTOs;
using Zariya.ViewModels;

namespace Zariya.Interfaces;

public interface IAuthService
{
    Task<AuthenticatedUserViewModel?> AuthenticateAsync(LoginDto login);
    Task<(bool Succeeded, string? ErrorMessage)> RegisterDonorAsync(RegisterDonorViewModel model);
}
