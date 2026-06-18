namespace Zariya.Interfaces;

public interface IDonationTypeService
{
    Task EnsureDefaultDonationTypesAsync();
    Task<int> GetDefaultDonationTypeIdAsync();
}
