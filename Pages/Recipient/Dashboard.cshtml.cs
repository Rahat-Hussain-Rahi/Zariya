using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Zariya.Pages
{
    public class RecipientDashboardModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public RecipientDashboardModel(ZariyaDbContext context)
        {
            _context = context;
        }

        public Patient? PatientProfile { get; set; }
        public List<Request> MyRequests { get; set; } = new List<Request>();
        public List<City> CitiesList { get; set; } = new List<City>();
        public List<string> BloodGroups { get; set; } = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        // Tab navigation
        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; } = string.Empty;

        // Settings tab properties
        [BindProperty(SupportsGet = true)]
        public string SettingsTab { get; set; } = "personal";
        public string Role { get; set; } = string.Empty;
        public User? CurrentUser { get; set; }
        public Zariya.Models.Donor? DonorProfile { get; set; }
        public List<Donation> Donations { get; set; } = new();
        public List<Zariya.Models.Request> SettingsRequests { get; set; } = new();
        public List<AdminLog> ActivityLogs { get; set; } = new();
        public List<User> AllUsers { get; set; } = new();
        public string SettingsNavPrefix { get; set; } = "/Recipient/Dashboard?tab=settings&settingsTab=";

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "patient" && role != "recipient")
            {
                return RedirectToPage("/Index");
            }

            if (Tab == "settings")
            {
                await LoadSettingsDataAsync(userId.Value);
            }
            else
            {
                await LoadDataAsync(userId.Value);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync(string phone, int cityId, string medicalCondition, string urgencyLevel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                patient.Phone = phone;
                patient.CityId = cityId;
                patient.MedicalCondition = medicalCondition;
                patient.UrgencyLevel = urgencyLevel;
                patient.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                SuccessMessage = "Recipient profile updated successfully.";
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostCreateRequestAsync(string bloodGroupNeeded, string urgency, string message)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                var request = new Request
                {
                    PatientId = patient.PatientId,
                    BloodGroupNeeded = bloodGroupNeeded,
                    Urgency = urgency,
                    Message = string.IsNullOrEmpty(message) ? "Urgent blood donation required." : message,
                    Status = "pending",
                    IsFulfilled = false,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Requests.Add(request);
                await _context.SaveChangesAsync();
                SuccessMessage = "Blood request submitted successfully. We have broadcasted it to matching donors.";
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostCancelRequestAsync(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null)
            {
                var request = await _context.Requests.FirstOrDefaultAsync(r => r.RequestId == requestId && r.PatientId == patient.PatientId);
                if (request != null)
                {
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                        {
                            try
                            {
                                // Delete donations
                            var donations = _context.Donations.Where(d => d.RequestId == requestId);
                            _context.Donations.RemoveRange(donations);

                            _context.Requests.Remove(request);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();

                            SuccessMessage = "Blood request cancelled and deleted successfully.";
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            ErrorMessage = $"Error deleting request: {ex.Message}";
                        }
                    }
                }
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        private async Task LoadDataAsync(int userId)
        {
            PatientProfile = await _context.Patients
                .Include(p => p.User)
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.UserId == userId);

            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

            if (PatientProfile != null)
            {
                MyRequests = await _context.Requests
                    .Include(r => r.Donor)
                    .ThenInclude(d => d!.City)
                    .Where(r => r.PatientId == PatientProfile.PatientId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
        }

        private async Task LoadSettingsDataAsync(int userId)
        {
            CurrentUser = await _context.Users.FindAsync(userId);
            if (CurrentUser == null) return;
            Role = CurrentUser.Role.Trim().ToLower();
            PatientProfile = await _context.Patients.Include(p => p.City).FirstOrDefaultAsync(p => p.UserId == userId);
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();
            if (SettingsTab == "request-history" && PatientProfile != null)
            {
                SettingsRequests = await _context.Requests
                    .Include(r => r.Donor)
                    .Where(r => r.PatientId == PatientProfile.PatientId)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
        }

        // --- Settings Handlers ---

        public async Task<IActionResult> OnPostUpdatePersonalInfoAsync(string fullName, string gender, string dateOfBirth, string bloodGroup)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "personal";
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");
            if (!string.IsNullOrWhiteSpace(fullName)) { user.FullName = fullName; HttpContext.Session.SetString("UserName", fullName); }
            var role = user.Role.Trim().ToLower();
            if (role is "patient" or "recipient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient != null) { if (!string.IsNullOrWhiteSpace(gender)) patient.Gender = gender; if (DateOnly.TryParse(dateOfBirth, out var dob)) patient.DateOfBirth = dob; if (!string.IsNullOrWhiteSpace(bloodGroup)) patient.BloodGroup = bloodGroup; patient.UpdatedAt = DateTime.Now; }
            }
            user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync();
            SuccessMessage = "Personal information updated successfully.";
            await LoadSettingsDataAsync(userId.Value); return Page();
        }

        public async Task<IActionResult> OnPostUpdateContactAsync(string phone, int? cityId, string? bio)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "contact";
            var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
            if (role is "patient" or "recipient") { var p = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == userId); if (p != null) { if (!string.IsNullOrWhiteSpace(phone)) p.Phone = phone; if (cityId.HasValue) p.CityId = cityId; p.UpdatedAt = DateTime.Now; } }
            await _context.SaveChangesAsync(); SuccessMessage = "Contact information updated successfully.";
            await LoadSettingsDataAsync(userId.Value); return Page();
        }

        public async Task<IActionResult> OnPostUpdateRequirementAsync(string medicalCondition, string urgencyLevel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "requirement";
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null) { patient.MedicalCondition = medicalCondition; patient.UrgencyLevel = urgencyLevel; patient.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = "Blood requirement details updated."; }
            await LoadSettingsDataAsync(userId.Value); return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "security";
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");
            if (user.PasswordHash != "HASHED_" + currentPassword && user.PasswordHash != currentPassword) { ErrorMessage = "Current password is incorrect."; await LoadSettingsDataAsync(userId.Value); return Page(); }
            if (newPassword != confirmPassword) { ErrorMessage = "New password and confirmation do not match."; await LoadSettingsDataAsync(userId.Value); return Page(); }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6) { ErrorMessage = "New password must be at least 6 characters."; await LoadSettingsDataAsync(userId.Value); return Page(); }
            user.PasswordHash = "HASHED_" + newPassword; user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync();
            SuccessMessage = "Password changed successfully."; await LoadSettingsDataAsync(userId.Value); return Page();
        }

        public async Task<IActionResult> OnPostDeactivateAccountAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            var user = await _context.Users.FindAsync(userId);
            if (user != null) { user.IsActive = false; user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); }
            HttpContext.Session.Clear(); return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostReportDonorAsync(int requestId, int reportedDonorId, string reason, string description)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var donor = await _context.Donors.Include(d => d.User).FirstOrDefaultAsync(d => d.DonorId == reportedDonorId);
            if (donor?.User == null)
            {
                ErrorMessage = "Reported donor not found.";
                await LoadDataAsync(userId.Value);
                return Page();
            }

            _context.Complaints.Add(new Complaint
            {
                ReporterId = userId.Value,
                ReportedUserId = donor.User.UserId,
                Reason = reason,
                Description = description,
                Status = "open",
                CreatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();
            SuccessMessage = "Your report has been submitted. The admin will review it shortly.";
            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            // This action is intentionally destructive, so we clean up dependent rows first.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.Include(u => u.Patient).FirstOrDefaultAsync(u => u.UserId == userId);
                if (user?.Patient != null)
                {
                    var patientId = user.Patient.PatientId;
                    var requests = await _context.Requests.Where(r => r.PatientId == patientId).ToListAsync();
                    foreach (var request in requests)
                    {
                        var donations = _context.Donations.Where(d => d.RequestId == request.RequestId);
                        _context.Donations.RemoveRange(donations);

                        _context.Requests.Remove(request);
                    }

                    var searchLogs = _context.SearchLogs.Where(sl => sl.PatientId == patientId);
                    _context.SearchLogs.RemoveRange(searchLogs);
                    var patientDonations = _context.Donations.Where(d => d.PatientId == patientId);
                    _context.Donations.RemoveRange(patientDonations);
                    _context.Patients.Remove(user.Patient);
                    _context.Users.Remove(user);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                HttpContext.Session.Clear();
                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ErrorMessage = $"Could not delete your account: {ex.Message}";
                await LoadDataAsync(userId.Value);
                return Page();
            }
        }
    }
}
