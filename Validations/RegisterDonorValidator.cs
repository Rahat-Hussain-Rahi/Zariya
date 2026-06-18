using Zariya.ViewModels;

namespace Zariya.Validations;

public sealed class RegisterDonorValidator
{
    public (bool Succeeded, string? ErrorMessage) Validate(RegisterDonorViewModel model)
    {
        if (string.IsNullOrWhiteSpace(model.FullName))
        {
            return (false, "Full name is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Email))
        {
            return (false, "Email is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Password) || model.Password.Length < 6)
        {
            return (false, "Password must be at least 6 characters long.");
        }

        if (string.IsNullOrWhiteSpace(model.Phone))
        {
            return (false, "Phone number is required.");
        }

        if (string.IsNullOrWhiteSpace(model.Cnic))
        {
            return (false, "CNIC is required.");
        }

        return (true, null);
    }
}
