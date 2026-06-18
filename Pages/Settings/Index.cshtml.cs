using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;
using DonorModel = Zariya.Models.Donor;
using PatientModel = Zariya.Models.Patient;
using UserModel = Zariya.Models.User;
using RequestModel = Zariya.Models.Request;

namespace Zariya.Pages.Settings;

public class IndexModel : PageModel
{
    private readonly ZariyaDbContext _context;

    public IndexModel(ZariyaDbContext context)
    {
        _context = context;
    }

    public string ActiveTab { get; set; } = "personal";
    public string Role { get; set; } = string.Empty;
    public List<City> CitiesList { get; set; } = new();
    public List<string> BloodGroups { get; set; } = new() { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

    public UserModel? CurrentUser { get; set; }
    public DonorModel? DonorProfile { get; set; }
    public PatientModel? PatientProfile { get; set; }
    public List<Donation> Donations { get; set; } = new();
    public List<RequestModel> Requests { get; set; } = new();
    public List<AdminLog> ActivityLogs { get; set; } = new();
    public List<UserModel> AllUsers { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(string tab = "personal")
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        ActiveTab = tab;
        await LoadDataAsync(userId.Value);
        return Page();
    }

    private async Task LoadDataAsync(int userId)
    {
        CurrentUser = await _context.Users.FindAsync(userId) as UserModel;
        if (CurrentUser == null) return;

        Role = CurrentUser.Role.Trim().ToLower();
        CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

        if (Role == "donor")
        {
            DonorProfile = await _context.Donors
                .Include(d => d.City)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (DonorProfile != null && ActiveTab == "donation-history")
            {
                Donations = await _context.Donations
                    .Include(d => d.Patient)
                    .Include(d => d.DonationType)
                    .Where(d => d.DonorId == DonorProfile.DonorId)
                    .OrderByDescending(d => d.DonationDate)
                    .ToListAsync();
            }
        }

        if (Role is "patient" or "recipient")
        {
            PatientProfile = await _context.Patients
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            if (PatientProfile != null && ActiveTab == "request-history")
            {
                Requests = await _context.Requests
                    .Include(r => r.Donor)
                    .Where(r => r.PatientId == PatientProfile.PatientId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
        }

        if (Role == "admin")
        {
            if (ActiveTab == "activity-logs")
            {
                ActivityLogs = await _context.AdminLogs
                    .Include(l => l.TargetUser)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(100)
                    .ToListAsync();
            }
            if (ActiveTab == "permissions")
            {
                AllUsers = await _context.Users
                    .OrderBy(u => u.FullName)
                    .ToListAsync();
            }
        }
    }

    public async Task<IActionResult> OnPostUpdatePersonalInfoAsync(string fullName, string gender, string dateOfBirth, string bloodGroup)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "personal";

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return RedirectToPage("/Account/Login");

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            user.FullName = fullName;
            HttpContext.Session.SetString("UserName", fullName);
        }

        var role = user.Role.Trim().ToLower();
        if (role == "donor")
        {
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                if (!string.IsNullOrWhiteSpace(gender)) donor.Gender = gender;
                if (DateOnly.TryParse(dateOfBirth, out var dob)) donor.DateOfBirth = dob;
                donor.UpdatedAt = DateTime.Now;
            }
        }
        else if (role is "patient" or "recipient")
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                if (!string.IsNullOrWhiteSpace(gender)) patient.Gender = gender;
                if (DateOnly.TryParse(dateOfBirth, out var dob)) patient.DateOfBirth = dob;
                if (!string.IsNullOrWhiteSpace(bloodGroup)) patient.BloodGroup = bloodGroup;
                patient.UpdatedAt = DateTime.Now;
            }
        }

        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        SuccessMessage = "Personal information updated successfully.";
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateContactAsync(string phone, int? cityId, string? bio)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "contact";

        var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
        if (role == "donor")
        {
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                if (!string.IsNullOrWhiteSpace(phone)) donor.Phone = phone;
                if (cityId.HasValue) donor.CityId = cityId;
                donor.Bio = bio;
                donor.UpdatedAt = DateTime.Now;
            }
        }
        else if (role is "patient" or "recipient")
        {
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                if (!string.IsNullOrWhiteSpace(phone)) patient.Phone = phone;
                if (cityId.HasValue) patient.CityId = cityId;
                patient.UpdatedAt = DateTime.Now;
            }
        }

        await _context.SaveChangesAsync();
        SuccessMessage = "Contact information updated successfully.";
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateMedicalAsync(string? bio)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "medical";
        var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
        if (donor != null)
        {
            donor.Bio = bio;
            donor.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            SuccessMessage = "Medical information updated.";
        }
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostUpdateRequirementAsync(string medicalCondition, string urgencyLevel)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "requirement";
        var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
        if (patient != null)
        {
            patient.MedicalCondition = medicalCondition;
            patient.UrgencyLevel = urgencyLevel;
            patient.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            SuccessMessage = "Blood requirement details updated.";
        }
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostToggleAvailabilityAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "availability";
        var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
        if (donor != null)
        {
            donor.Availability = donor.Availability == "Available" ? "Unavailable" : "Available";
            donor.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            SuccessMessage = $"Availability set to {donor.Availability}.";
        }
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "security";

        var user = await _context.Users.FindAsync(userId);
        if (user == null) return RedirectToPage("/Account/Login");

        if (user.PasswordHash != "HASHED_" + currentPassword && user.PasswordHash != currentPassword)
        {
            ErrorMessage = "Current password is incorrect.";
            await LoadDataAsync(userId.Value);
            return Page();
        }

        if (newPassword != confirmPassword)
        {
            ErrorMessage = "New password and confirmation do not match.";
            await LoadDataAsync(userId.Value);
            return Page();
        }

        if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
        {
            ErrorMessage = "New password must be at least 6 characters.";
            await LoadDataAsync(userId.Value);
            return Page();
        }

        user.PasswordHash = "HASHED_" + newPassword;
        user.UpdatedAt = DateTime.Now;
        await _context.SaveChangesAsync();
        SuccessMessage = "Password changed successfully.";
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostDeactivateAccountAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "danger";

        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.IsActive = false;
            user.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostDeleteAccountAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        ActiveTab = "danger";

        var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
        var user = await _context.Users
            .Include(u => u.Donor)
            .Include(u => u.Patient)
            .FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null) return RedirectToPage("/Account/Login");

        if (role == "donor" && user.Donor != null)
        {
            var donorId = user.Donor.DonorId;
            _context.Donations.RemoveRange(_context.Donations.Where(d => d.DonorId == donorId));
            _context.DonorAvailabilityLogs.RemoveRange(_context.DonorAvailabilityLogs.Where(l => l.DonorId == donorId));
            var requests = _context.Requests.Where(r => r.DonorId == donorId);
            foreach (var r in requests) { r.DonorId = null; r.Status = "pending"; r.DonorResponse = null; r.RespondedAt = null; }
            _context.Donors.Remove(user.Donor);
        }
        else if (role is "patient" or "recipient" && user.Patient != null)
        {
            var patientId = user.Patient.PatientId;
            _context.SearchLogs.RemoveRange(_context.SearchLogs.Where(s => s.PatientId == patientId));
            var reqs = _context.Requests.Where(r => r.PatientId == patientId).ToList();
            foreach (var r in reqs)
            {
                _context.Donations.RemoveRange(_context.Donations.Where(d => d.RequestId == r.RequestId));
                _context.Requests.Remove(r);
            }
            _context.Patients.Remove(user.Patient);
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        HttpContext.Session.Clear();
        return RedirectToPage("/Index");
    }

    public async Task<IActionResult> OnPostUpdateUserRoleAsync(int targetUserId, string newRole)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
        if (role != "admin") return RedirectToPage("/Index");
        ActiveTab = "permissions";

        var target = await _context.Users.FindAsync(targetUserId);
        if (target != null && !string.IsNullOrWhiteSpace(newRole))
        {
            target.Role = newRole;
            target.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            SuccessMessage = $"User {target.FullName} role updated to {newRole}.";
        }
        await LoadDataAsync(userId.Value);
        return Page();
    }

    public async Task<IActionResult> OnPostToggleUserActiveAsync(int targetUserId)
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");
        var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
        if (role != "admin") return RedirectToPage("/Index");
        ActiveTab = "permissions";

        var target = await _context.Users.FindAsync(targetUserId);
        if (target != null)
        {
            target.IsActive = !target.IsActive;
            target.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            SuccessMessage = $"User {target.FullName} {(target.IsActive ? "activated" : "deactivated")}.";
        }
        await LoadDataAsync(userId.Value);
        return Page();
    }
}
