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
    public class DonorDashboardModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public DonorDashboardModel(ZariyaDbContext context)
        {
            _context = context;
        }

        // Main section
        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; } = string.Empty;

        public Zariya.Models.Donor? DonorProfile { get; set; }
        public List<Request> AssociatedRequests { get; set; } = new List<Request>();
        public List<Request> MatchingRequests { get; set; } = new List<Request>();
        public List<City> CitiesList { get; set; } = new List<City>();
        public List<string> BloodGroups { get; set; } = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

        public int DaysSinceLastDonation { get; set; } = 45;
        public int EligibilityRemainingDays { get; set; } = 45;
        public DateOnly NextEligibilityDate { get; set; }

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        // Settings tab properties
        [BindProperty(SupportsGet = true)]
        public string SettingsTab { get; set; } = "personal";
        public string Role { get; set; } = string.Empty;
        public User? CurrentUser { get; set; }
        public Patient? PatientProfile { get; set; }
        public List<Donation> Donations { get; set; } = new();
        public List<Zariya.Models.Request> SettingsRequests { get; set; } = new();
        public List<AdminLog> ActivityLogs { get; set; } = new();
        public List<User> AllUsers { get; set; } = new();
        public string SettingsNavPrefix { get; set; } = "/Donor/Dashboard?tab=settings&settingsTab=";

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Account/Login");
            }

            var role = HttpContext.Session.GetString("UserRole");
            if (role != "donor")
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

        public async Task<IActionResult> OnPostToggleAvailabilityAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                donor.Availability = donor.Availability == "Available" ? "Unavailable" : "Available";
                donor.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                SuccessMessage = $"Your availability is now set to: {donor.Availability}.";
            }

            if (Tab == "settings") { SettingsTab = "availability"; await LoadSettingsDataAsync(userId.Value); }
            else { await LoadDataAsync(userId.Value); }
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync(string phone, int cityId, string bio)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                donor.Phone = phone;
                donor.CityId = cityId;
                donor.Bio = bio;
                donor.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();
                SuccessMessage = "Profile updated successfully.";
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostAcceptRequestAsync(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request != null)
                {
                    request.DonorId = donor.DonorId;
                    request.Status = "accepted";
                    request.RespondedAt = DateTime.Now;
                    request.DonorResponse = "Accepted request to donate blood.";
                    request.UpdatedAt = DateTime.Now;

                    var donationTypeId = await GetDefaultDonationTypeIdAsync();

                    // Insert record in Donations
                    var donation = new Donation
                    {
                        DonorId = donor.DonorId,
                        PatientId = request.PatientId,
                        RequestId = request.RequestId,
                        DonationDate = DateOnly.FromDateTime(DateTime.Now),
                        CityId = donor.CityId,
                        HospitalName = "Local Medical Facility",
                        IsVerified = false,
                        Notes = "Assigned via Zariya Portal",
                        DonationTypeId = donationTypeId,
                        CreatedAt = DateTime.Now
                    };
                    _context.Donations.Add(donation);

                    donor.TotalDonations += 1;
                    donor.LastDonated = DateOnly.FromDateTime(DateTime.Now);

                    await _context.SaveChangesAsync();
                    SuccessMessage = "You have successfully accepted the request. Thank you for your donation!";
                }
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostDeclineRequestAsync(int requestId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null)
            {
                var request = await _context.Requests.FindAsync(requestId);
                if (request != null)
                {
                    if (request.DonorId == donor.DonorId)
                    {
                        request.DonorId = null;
                        request.Status = "pending";
                    }
                    else
                    {
                        request.Status = "declined";
                    }
                    request.UpdatedAt = DateTime.Now;
                    await _context.SaveChangesAsync();
                    SuccessMessage = "Request declined.";
                }
            }

            await LoadDataAsync(userId.Value);
            return Page();
        }

        public async Task<IActionResult> OnPostReportRequestAsync(int requestId, int reportedPatientId, string reason, string description)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientId == reportedPatientId);
            if (patient?.User == null)
            {
                ErrorMessage = "Reported patient not found.";
                await LoadDataAsync(userId.Value);
                return Page();
            }

            _context.Complaints.Add(new Complaint
            {
                ReporterId = userId.Value,
                ReportedUserId = patient.User.UserId,
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

            // Delete the current donor account and keep related requests in a safe pending state.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.Include(u => u.Donor).FirstOrDefaultAsync(u => u.UserId == userId);
                if (user?.Donor != null)
                {
                    var donorId = user.Donor.DonorId;
                    var donorDonations = _context.Donations.Where(d => d.DonorId == donorId);
                    _context.Donations.RemoveRange(donorDonations);

                    var donorRequests = _context.Requests.Where(r => r.DonorId == donorId);
                    foreach (var request in donorRequests)
                    {
                        request.DonorId = null;
                        request.Status = "pending";
                        request.DonorResponse = null;
                        request.RespondedAt = null;
                        request.UpdatedAt = DateTime.Now;
                    }

                    var donorLogs = _context.DonorAvailabilityLogs.Where(l => l.DonorId == donorId);
                    _context.DonorAvailabilityLogs.RemoveRange(donorLogs);
                    _context.Donors.Remove(user.Donor);
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

        private async Task<int> GetDefaultDonationTypeIdAsync()
        {
            var donationType = await _context.DonationTypes
                .OrderByDescending(dt => dt.TypeName == "Whole Blood")
                .ThenBy(dt => dt.TypeId)
                .FirstOrDefaultAsync(dt => dt.IsActive);

            if (donationType != null)
            {
                return donationType.TypeId;
            }

            donationType = new DonationType
            {
                TypeName = "Whole Blood",
                Description = "Standard whole blood donation",
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            _context.DonationTypes.Add(donationType);
            await _context.SaveChangesAsync();
            return donationType.TypeId;
        }

        private async Task LoadDataAsync(int userId)
        {
            DonorProfile = await _context.Donors
                .Include(d => d.User)
                .Include(d => d.City)
                .FirstOrDefaultAsync(d => d.UserId == userId);

            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

            if (DonorProfile != null)
            {
                if (DonorProfile.LastDonated.HasValue)
                {
                    var today = DateOnly.FromDateTime(DateTime.Today);
                    var days = today.DayNumber - DonorProfile.LastDonated.Value.DayNumber;
                    DaysSinceLastDonation = Math.Max(0, days);
                    EligibilityRemainingDays = Math.Max(0, 90 - DaysSinceLastDonation);
                    NextEligibilityDate = DonorProfile.LastDonated.Value.AddDays(90);
                }
                else
                {
                    DaysSinceLastDonation = 120;
                    EligibilityRemainingDays = 0;
                    NextEligibilityDate = DateOnly.FromDateTime(DateTime.Today);
                }

                AssociatedRequests = await _context.Requests
                    .Include(r => r.Patient)
                    .ThenInclude(p => p.City)
                    .Where(r => r.DonorId == DonorProfile.DonorId)
                    .OrderByDescending(r => r.UpdatedAt)
                    .ToListAsync();

                MatchingRequests = await _context.Requests
                    .Include(r => r.Patient)
                    .ThenInclude(p => p.City)
                    .Where(r => r.Status == "pending" && r.BloodGroupNeeded == DonorProfile.BloodGroup && r.DonorId == null)
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
        }

        private async Task LoadSettingsDataAsync(int userId)
        {
            CurrentUser = await _context.Users.FindAsync(userId);
            if (CurrentUser == null) return;
            Role = CurrentUser.Role.Trim().ToLower();
            DonorProfile = await _context.Donors.Include(d => d.City).FirstOrDefaultAsync(d => d.UserId == userId);
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();
            if (SettingsTab == "donation-history" && DonorProfile != null)
            {
                Donations = await _context.Donations
                    .Include(d => d.Patient)
                    .Include(d => d.DonationType)
                    .Where(d => d.DonorId == DonorProfile.DonorId)
                    .OrderByDescending(d => d.DonationDate)
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
            if (role == "donor")
            {
                var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
                if (donor != null) { if (!string.IsNullOrWhiteSpace(gender)) donor.Gender = gender; if (DateOnly.TryParse(dateOfBirth, out var dob)) donor.DateOfBirth = dob; donor.UpdatedAt = DateTime.Now; }
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
            if (role == "donor") { var d = await _context.Donors.FirstOrDefaultAsync(x => x.UserId == userId); if (d != null) { if (!string.IsNullOrWhiteSpace(phone)) d.Phone = phone; if (cityId.HasValue) d.CityId = cityId; d.Bio = bio; d.UpdatedAt = DateTime.Now; } }
            await _context.SaveChangesAsync(); SuccessMessage = "Contact information updated successfully.";
            await LoadSettingsDataAsync(userId.Value); return Page();
        }

        public async Task<IActionResult> OnPostUpdateMedicalAsync(string? bio)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "medical";
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null) { donor.Bio = bio; donor.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = "Medical information updated."; }
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
    }
}
