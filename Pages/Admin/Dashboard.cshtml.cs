using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zariya.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public DashboardModel(ZariyaDbContext context)
        {
            _context = context;
        }

        // Time filter selection (Last 24h, 7 Days, 30 Days)
        [BindProperty(SupportsGet = true)]
        public string TimeFilter { get; set; } = "24h";

        [BindProperty(SupportsGet = true)]
        public string Tab { get; set; } = "overview";

        public List<Zariya.Models.Donor> DonorsList { get; set; } = new List<Zariya.Models.Donor>();
        public List<Patient> PatientsList { get; set; } = new List<Patient>();
        public List<User> UsersList { get; set; } = new List<User>();

        // KPI Counts
        public int TotalDonors { get; set; }
        public int TotalRecipients { get; set; }
        public int ActiveRequests { get; set; }
        public int AvailableDonors { get; set; }
        public int VerifiedUsers { get; set; }
        public int DonationsDone { get; set; }

        // Chart Data
        public Dictionary<string, int> BloodGroupDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> ChartPercentages { get; set; } = new Dictionary<string, double>();

        // Verification Queue
        public List<QueueItem> VerificationQueue { get; set; } = new List<QueueItem>();

        // Complaints
        public List<Complaint> ComplaintsList { get; set; } = new List<Complaint>();
        public int PendingComplaintsCount { get; set; }

        // Contact Messages
        public List<ContactMessage> ContactMessagesList { get; set; } = new List<ContactMessage>();
        public int UnreadMessagesCount { get; set; }

        // Settings tab
        [BindProperty(SupportsGet = true)]
        public string SettingsTab { get; set; } = "personal";
        public string Role { get; set; } = string.Empty;
        public User? CurrentUser { get; set; }
        public Zariya.Models.Donor? DonorProfile { get; set; }
        public Patient? PatientProfile { get; set; }
        public List<Donation> Donations { get; set; } = new();
        public List<Zariya.Models.Request> SettingsRequests { get; set; } = new();
        public List<AdminLog> ActivityLogs { get; set; } = new();
        public List<User> AllUsers { get; set; } = new();
        public string SettingsNavPrefix { get; set; } = "/Admin/Dashboard?tab=settings&settingsTab=";

        // Dropdowns for broadcast
        public List<City> CitiesList { get; set; } = new List<City>();
        public List<string> BloodGroups { get; set; } = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

        // System messages
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        // Static list to persist broadcasts across requests during demonstration
        private static readonly List<TimelineEvent> _broadcastEvents = new List<TimelineEvent>();

        public List<TimelineEvent> SystemLiveFeed { get; set; } = new List<TimelineEvent>();

        public async Task OnGetAsync()
        {
            await LoadDashboardDataAsync();
            if (Tab == "donors")
            {
                DonorsList = await _context.Donors.Include(d => d.User).Include(d => d.City).OrderByDescending(d => d.DonorId).ToListAsync();
            }
            else if (Tab == "patients")
            {
                PatientsList = await _context.Patients.Include(p => p.User).Include(p => p.City).OrderByDescending(p => p.PatientId).ToListAsync();
            }
            else if (Tab == "users")
            {
                UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
            }
            else if (Tab == "complaints")
            {
                ComplaintsList = await _context.Complaints
                    .Include(c => c.Reporter)
                    .Include(c => c.ReportedUser)
                    .Include(c => c.ResolvedByNavigation)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();
            }
            else if (Tab == "messages")
            {
                ContactMessagesList = await _context.ContactMessages
                    .OrderByDescending(m => m.CreatedAt)
                    .ToListAsync();
            }
            else if (Tab == "settings")
            {
                await LoadSettingsDataAsync();
            }
        }

        public async Task<IActionResult> OnPostActivateBroadcastAsync(string broadcastBloodGroup, int broadcastCityId, string broadcastMessage)
        {
            if (string.IsNullOrEmpty(broadcastBloodGroup) || broadcastCityId == 0 || string.IsNullOrEmpty(broadcastMessage))
            {
                ErrorMessage = "Please fill in all the broadcast fields.";
                await LoadDashboardDataAsync();
                return Page();
            }

            var city = await _context.Cities.FindAsync(broadcastCityId);
            var cityName = city?.CityName ?? "Unknown";

            // Log the search/broadcast in Search_Logs
            var searchLog = new SearchLog
            {
                BloodGroupFilter = broadcastBloodGroup,
                SearchCityId = broadcastCityId,
                ResultsCount = (short)await _context.Donors.CountAsync(d => d.BloodGroup == broadcastBloodGroup && d.CityId == broadcastCityId),
                CreatedAt = DateTime.Now
            };
            _context.SearchLogs.Add(searchLog);
            await _context.SaveChangesAsync();

            // Add item to demo live feed list
            _broadcastEvents.Insert(0, new TimelineEvent
            {
                Time = DateTime.Now.ToString("hh:mm tt"),
                Title = "Broadcast Successful",
                Description = $"{searchLog.ResultsCount} donors notified in {cityName} for {broadcastBloodGroup} request.",
                ColorClass = "red"
            });

            SuccessMessage = $"Broadcast activated successfully! Notified {searchLog.ResultsCount} matching donors in {cityName}.";
            await LoadDashboardDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostVerifyDonorAsync(int donorId)
        {
            var donor = await _context.Donors.FindAsync(donorId);
            if (donor != null)
            {
                donor.IsAdminVerified = true;
                donor.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                _broadcastEvents.Insert(0, new TimelineEvent
                {
                    Time = DateTime.Now.ToString("hh:mm tt"),
                    Title = "Donor Verified",
                    Description = $"Administrator verified profile and CNIC of {donor.FullName}.",
                    ColorClass = "green"
                });

                SuccessMessage = $"Donor {donor.FullName} has been verified successfully.";
            }
            else
            {
                ErrorMessage = "Donor record not found.";
            }

            await LoadDashboardDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRejectDonorAsync(int donorId)
        {
            var donor = await _context.Donors.FindAsync(donorId);
            if (donor != null)
            {
                donor.IsAdminVerified = false;
                donor.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                _broadcastEvents.Insert(0, new TimelineEvent
                {
                    Time = DateTime.Now.ToString("hh:mm tt"),
                    Title = "Donor Rejected",
                    Description = $"Administrator rejected verification for {donor.FullName}.",
                    ColorClass = "red"
                });

                SuccessMessage = $"Donor {donor.FullName} has been rejected.";
            }
            else
            {
                ErrorMessage = "Donor record not found.";
            }

            await LoadDashboardDataAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditDonorAsync(int editDonorId, string fullName, string phone, string bloodGroup, string availability, int cityId, bool isAdminVerified)
        {
            var donor = await _context.Donors.Include(d => d.User).FirstOrDefaultAsync(d => d.DonorId == editDonorId);
            if (donor != null)
            {
                donor.FullName = fullName;
                donor.Phone = phone;
                donor.BloodGroup = bloodGroup;
                donor.Availability = availability;
                donor.CityId = cityId;
                donor.IsAdminVerified = isAdminVerified;
                donor.UpdatedAt = DateTime.Now;

                if (donor.User != null)
                {
                    donor.User.FullName = fullName;
                    donor.User.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                SuccessMessage = $"Donor {fullName} updated successfully.";
            }
            else
            {
                ErrorMessage = "Donor not found.";
            }

            Tab = "donors";
            await LoadDashboardDataAsync();
            DonorsList = await _context.Donors.Include(d => d.User).Include(d => d.City).OrderByDescending(d => d.DonorId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteDonorAsync(int deleteDonorId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var donor = await _context.Donors.Include(d => d.User).FirstOrDefaultAsync(d => d.DonorId == deleteDonorId);
                    if (donor != null)
                    {
                        // Delete related donations
                        var donations = _context.Donations.Where(d => d.DonorId == deleteDonorId);
                        _context.Donations.RemoveRange(donations);

                        // Reset requests assigned to this donor
                        var requests = _context.Requests.Where(r => r.DonorId == deleteDonorId);
                        foreach (var r in requests)
                        {
                            r.DonorId = null;
                            r.Status = "pending";
                        }

                        // Delete availability logs
                        var logs = _context.DonorAvailabilityLogs.Where(l => l.DonorId == deleteDonorId);
                        _context.DonorAvailabilityLogs.RemoveRange(logs);

                        // Remove donor profile
                        _context.Donors.Remove(donor);

                        // Remove associated user record
                        if (donor.User != null)
                        {
                            _context.Users.Remove(donor.User);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        SuccessMessage = $"Donor {donor.FullName} and their user account have been deleted.";
                    }
                    else
                    {
                        ErrorMessage = "Donor not found.";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ErrorMessage = $"Error deleting donor: {ex.Message}";
                }
            }

            Tab = "donors";
            await LoadDashboardDataAsync();
            DonorsList = await _context.Donors.Include(d => d.User).Include(d => d.City).OrderByDescending(d => d.DonorId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostEditPatientAsync(int editPatientId, string fullName, string phone, string bloodGroup, string medicalCondition, string urgencyLevel, int cityId)
        {
            var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientId == editPatientId);
            if (patient != null)
            {
                patient.FullName = fullName;
                patient.Phone = phone;
                patient.BloodGroup = bloodGroup;
                patient.MedicalCondition = medicalCondition;
                patient.UrgencyLevel = urgencyLevel;
                patient.CityId = cityId;
                patient.UpdatedAt = DateTime.Now;

                if (patient.User != null)
                {
                    patient.User.FullName = fullName;
                    patient.User.UpdatedAt = DateTime.Now;
                }

                await _context.SaveChangesAsync();
                SuccessMessage = $"Recipient {fullName} updated successfully.";
            }
            else
            {
                ErrorMessage = "Recipient not found.";
            }

            Tab = "patients";
            await LoadDashboardDataAsync();
            PatientsList = await _context.Patients.Include(p => p.User).Include(p => p.City).OrderByDescending(p => p.PatientId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeletePatientAsync(int deletePatientId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var patient = await _context.Patients.Include(p => p.User).FirstOrDefaultAsync(p => p.PatientId == deletePatientId);
                    if (patient != null)
                    {
                        // Find requests
                        var requests = await _context.Requests.Where(r => r.PatientId == deletePatientId).ToListAsync();
                        foreach (var r in requests)
                        {
                            // Delete donations
                            var donations = _context.Donations.Where(d => d.RequestId == r.RequestId);
                            _context.Donations.RemoveRange(donations);

                            _context.Requests.Remove(r);
                        }

                        // Delete search logs
                        var searchLogs = _context.SearchLogs.Where(sl => sl.PatientId == deletePatientId);
                        _context.SearchLogs.RemoveRange(searchLogs);

                        // Delete donations associated with patient
                        var patientDonations = _context.Donations.Where(d => d.PatientId == deletePatientId);
                        _context.Donations.RemoveRange(patientDonations);

                        // Remove patient profile
                        _context.Patients.Remove(patient);

                        // Remove associated user record
                        if (patient.User != null)
                        {
                            _context.Users.Remove(patient.User);
                        }

                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        SuccessMessage = $"Recipient {patient.FullName} and their user account have been deleted.";
                    }
                    else
                    {
                        ErrorMessage = "Recipient not found.";
                    }
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ErrorMessage = $"Error deleting recipient: {ex.Message}";
                }
            }

            Tab = "patients";
            await LoadDashboardDataAsync();
            PatientsList = await _context.Patients.Include(p => p.User).Include(p => p.City).OrderByDescending(p => p.PatientId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateUserAsync(string fullName, string email, string password, string role)
        {
            // Keep the admin-created account flow simple: create a login and let the existing
            // database trigger/provisioning logic decide whether a profile row is needed.
            role = (role ?? string.Empty).Trim().ToLower();
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                ErrorMessage = "Please fill in all user fields.";
                Tab = "users";
                await LoadDashboardDataAsync();
                UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
                return Page();
            }

            if (await _context.Users.AnyAsync(u => u.Email == email))
            {
                ErrorMessage = "A user with this email already exists.";
                Tab = "users";
                await LoadDashboardDataAsync();
                UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
                return Page();
            }

            var allowedRoles = new[] { "admin", "donor", "patient", "recipient" };
            if (!allowedRoles.Contains(role))
            {
                ErrorMessage = "Please choose a valid user role.";
                Tab = "users";
                await LoadDashboardDataAsync();
                UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
                return Page();
            }

            _context.Users.Add(new User
            {
                FullName = fullName,
                Email = email,
                PasswordHash = "HASHED_" + password,
                Role = role,
                IsEmailVerified = true,
                IsActive = true,
                FailedAttempts = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            });
            await _context.SaveChangesAsync();

            SuccessMessage = $"User {fullName} added successfully.";
            Tab = "users";
            await LoadDashboardDataAsync();
            UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUserAsync(int deleteUserId)
        {
            // This cleanup keeps the database consistent if admin removes a user account directly.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users
                    .Include(u => u.Donor)
                    .Include(u => u.Patient)
                    .FirstOrDefaultAsync(u => u.UserId == deleteUserId);

                if (user == null)
                {
                    ErrorMessage = "User not found.";
                }
                else
                {
                    if (user.Donor != null)
                    {
                        var donorId = user.Donor.DonorId;
                        var donorDonations = _context.Donations.Where(d => d.DonorId == donorId);
                        _context.Donations.RemoveRange(donorDonations);

                        var donorRequests = _context.Requests.Where(r => r.DonorId == donorId);
                        foreach (var request in donorRequests)
                        {
                            request.DonorId = null;
                            request.Status = "pending";
                        }

                        var donorLogs = _context.DonorAvailabilityLogs.Where(l => l.DonorId == donorId);
                        _context.DonorAvailabilityLogs.RemoveRange(donorLogs);
                        _context.Donors.Remove(user.Donor);
                    }

                    if (user.Patient != null)
                    {
                        var patientId = user.Patient.PatientId;
                        var patientRequests = await _context.Requests.Where(r => r.PatientId == patientId).ToListAsync();
                        foreach (var request in patientRequests)
                        {
                            var requestDonations = _context.Donations.Where(d => d.RequestId == request.RequestId);
                            _context.Donations.RemoveRange(requestDonations);

                            _context.Requests.Remove(request);
                        }

                        var searchLogs = _context.SearchLogs.Where(sl => sl.PatientId == patientId);
                        _context.SearchLogs.RemoveRange(searchLogs);
                        var patientDonations = _context.Donations.Where(d => d.PatientId == patientId);
                        _context.Donations.RemoveRange(patientDonations);
                        _context.Patients.Remove(user.Patient);
                    }

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    SuccessMessage = "User deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                ErrorMessage = $"Error deleting user: {ex.Message}";
            }

            Tab = "users";
            await LoadDashboardDataAsync();
            UsersList = await _context.Users.OrderByDescending(u => u.UserId).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostResolveComplaintAsync(int complaintId, string? adminNotes)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");

            var complaint = await _context.Complaints.FindAsync(complaintId);
            if (complaint != null)
            {
                complaint.Status = "resolved";
                complaint.AdminNotes = adminNotes;
                complaint.ResolvedBy = userId;
                complaint.ResolvedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                SuccessMessage = "Complaint marked as resolved.";
            }

            Tab = "complaints";
            await LoadDashboardDataAsync();
            ComplaintsList = await _context.Complaints
                .Include(c => c.Reporter)
                .Include(c => c.ReportedUser)
                .Include(c => c.ResolvedByNavigation)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostMarkMessageReadAsync(int messageId)
        {
            var msg = await _context.ContactMessages.FindAsync(messageId);
            if (msg != null)
            {
                msg.IsRead = true;
                await _context.SaveChangesAsync();
            }
            Tab = "messages";
            await LoadDashboardDataAsync();
            ContactMessagesList = await _context.ContactMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteMessageAsync(int messageId)
        {
            var msg = await _context.ContactMessages.FindAsync(messageId);
            if (msg != null)
            {
                _context.ContactMessages.Remove(msg);
                await _context.SaveChangesAsync();
                SuccessMessage = "Message deleted.";
            }
            Tab = "messages";
            await LoadDashboardDataAsync();
            ContactMessagesList = await _context.ContactMessages.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return Page();
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
            else if (role is "patient" or "recipient")
            {
                var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
                if (patient != null) { if (!string.IsNullOrWhiteSpace(gender)) patient.Gender = gender; if (DateOnly.TryParse(dateOfBirth, out var dob)) patient.DateOfBirth = dob; if (!string.IsNullOrWhiteSpace(bloodGroup)) patient.BloodGroup = bloodGroup; patient.UpdatedAt = DateTime.Now; }
            }
            user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync();
            SuccessMessage = "Personal information updated successfully.";
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostUpdateContactAsync(string phone, int? cityId, string? bio)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "contact";
            var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
            if (role == "donor") { var d = await _context.Donors.FirstOrDefaultAsync(x => x.UserId == userId); if (d != null) { if (!string.IsNullOrWhiteSpace(phone)) d.Phone = phone; if (cityId.HasValue) d.CityId = cityId; d.Bio = bio; d.UpdatedAt = DateTime.Now; } }
            else if (role is "patient" or "recipient") { var p = await _context.Patients.FirstOrDefaultAsync(x => x.UserId == userId); if (p != null) { if (!string.IsNullOrWhiteSpace(phone)) p.Phone = phone; if (cityId.HasValue) p.CityId = cityId; p.UpdatedAt = DateTime.Now; } }
            await _context.SaveChangesAsync(); SuccessMessage = "Contact information updated successfully.";
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostUpdateMedicalAsync(string? bio)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "medical";
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null) { donor.Bio = bio; donor.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = "Medical information updated."; }
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostUpdateRequirementAsync(string medicalCondition, string urgencyLevel)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "requirement";
            var patient = await _context.Patients.FirstOrDefaultAsync(p => p.UserId == userId);
            if (patient != null) { patient.MedicalCondition = medicalCondition; patient.UrgencyLevel = urgencyLevel; patient.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = "Blood requirement details updated."; }
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostToggleAvailabilityAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "availability";
            var donor = await _context.Donors.FirstOrDefaultAsync(d => d.UserId == userId);
            if (donor != null) { donor.Availability = donor.Availability == "Available" ? "Unavailable" : "Available"; donor.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = $"Availability set to {donor.Availability}."; }
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync(string currentPassword, string newPassword, string confirmPassword)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            Tab = "settings"; SettingsTab = "security";
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return RedirectToPage("/Account/Login");
            if (user.PasswordHash != "HASHED_" + currentPassword && user.PasswordHash != currentPassword) { ErrorMessage = "Current password is incorrect."; await LoadSettingsDataAsync(); return Page(); }
            if (newPassword != confirmPassword) { ErrorMessage = "New password and confirmation do not match."; await LoadSettingsDataAsync(); return Page(); }
            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6) { ErrorMessage = "New password must be at least 6 characters."; await LoadSettingsDataAsync(); return Page(); }
            user.PasswordHash = "HASHED_" + newPassword; user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync();
            SuccessMessage = "Password changed successfully."; await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostDeactivateAccountAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            var user = await _context.Users.FindAsync(userId);
            if (user != null) { user.IsActive = false; user.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); }
            HttpContext.Session.Clear(); return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostDeleteAccountAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
            var user = await _context.Users.Include(u => u.Donor).Include(u => u.Patient).FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null) return RedirectToPage("/Account/Login");
            if (role == "donor" && user.Donor != null)
            {
                var donorId = user.Donor.DonorId;
                _context.Donations.RemoveRange(_context.Donations.Where(d => d.DonorId == donorId));
                _context.DonorAvailabilityLogs.RemoveRange(_context.DonorAvailabilityLogs.Where(l => l.DonorId == donorId));
                foreach (var r in _context.Requests.Where(r => r.DonorId == donorId)) { r.DonorId = null; r.Status = "pending"; r.DonorResponse = null; r.RespondedAt = null; }
                _context.Donors.Remove(user.Donor);
            }
            else if (role is "patient" or "recipient" && user.Patient != null)
            {
                var patientId = user.Patient.PatientId;
                _context.SearchLogs.RemoveRange(_context.SearchLogs.Where(s => s.PatientId == patientId));
                foreach (var r in _context.Requests.Where(r => r.PatientId == patientId).ToList())
                {
                    _context.Donations.RemoveRange(_context.Donations.Where(d => d.RequestId == r.RequestId));
                    _context.Requests.Remove(r);
                }
                _context.Patients.Remove(user.Patient);
            }
            _context.Users.Remove(user); await _context.SaveChangesAsync();
            HttpContext.Session.Clear(); return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnPostUpdateUserRoleAsync(int targetUserId, string newRole)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
            if (role != "admin") return RedirectToPage("/Index");
            Tab = "settings"; SettingsTab = "permissions";
            var target = await _context.Users.FindAsync(targetUserId);
            if (target != null && !string.IsNullOrWhiteSpace(newRole)) { target.Role = newRole; target.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = $"User {target.FullName} role updated to {newRole}."; }
            await LoadSettingsDataAsync(); return Page();
        }

        public async Task<IActionResult> OnPostToggleUserActiveAsync(int targetUserId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Account/Login");
            var role = HttpContext.Session.GetString("UserRole")?.Trim().ToLower();
            if (role != "admin") return RedirectToPage("/Index");
            Tab = "settings"; SettingsTab = "permissions";
            var target = await _context.Users.FindAsync(targetUserId);
            if (target != null) { target.IsActive = !target.IsActive; target.UpdatedAt = DateTime.Now; await _context.SaveChangesAsync(); SuccessMessage = $"User {target.FullName} {(target.IsActive ? "activated" : "deactivated")}."; }
            await LoadSettingsDataAsync(); return Page();
        }

        private async Task LoadSettingsDataAsync()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return;
            CurrentUser = await _context.Users.FindAsync(userId);
            if (CurrentUser == null) return;
            Role = CurrentUser.Role.Trim().ToLower();
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();
            if (SettingsTab == "activity-logs")
            {
                ActivityLogs = await _context.AdminLogs
                    .Include(l => l.TargetUser)
                    .OrderByDescending(l => l.CreatedAt)
                    .Take(100)
                    .ToListAsync();
            }
            if (SettingsTab == "permissions")
            {
                AllUsers = await _context.Users.OrderBy(u => u.FullName).ToListAsync();
            }
        }

        private async Task LoadDashboardDataAsync()
        {
            // Load current user and role for sidebar settings link
            var sid = HttpContext.Session.GetInt32("UserId");
            if (sid != null)
            {
                var cu = await _context.Users.FindAsync(sid.Value);
                if (cu != null)
                {
                    Role = cu.Role.Trim().ToLower();
                    CurrentUser = cu;
                }
            }

            // Fetch cities for broadcast form
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

            // Calculate stats from actual database
            TotalDonors = await _context.Donors.CountAsync();
            TotalRecipients = await _context.Patients.CountAsync();
            ActiveRequests = await _context.Requests.CountAsync(r => r.Status == "pending" || r.Status == "accepted");
            AvailableDonors = await _context.Donors.CountAsync(d => d.Availability == "Available");
            VerifiedUsers = await _context.Users.CountAsync(u => u.IsEmailVerified == true);
            DonationsDone = await _context.Donations.CountAsync();
            PendingComplaintsCount = await _context.Complaints.CountAsync(c => c.Status == "open");
            UnreadMessagesCount = await _context.ContactMessages.CountAsync(m => !m.IsRead);

            // Blood Group Distribution from actual donor records
            var dbDistribution = await _context.Donors
                .GroupBy(d => d.BloodGroup)
                .Select(g => new { BloodGroup = g.Key, Count = g.Count() })
                .ToListAsync();

            foreach (var bg in BloodGroups)
            {
                BloodGroupDistribution[bg] = dbDistribution.FirstOrDefault(x => x.BloodGroup == bg)?.Count ?? 0;
            }

            // Find maximum count for scaling bar charts
            double maxVal = BloodGroupDistribution.Values.Max();
            if (maxVal == 0) maxVal = 1;
            foreach (var item in BloodGroupDistribution)
            {
                // Percentage height (capped at 100%)
                ChartPercentages[item.Key] = Math.Min(100.0, (item.Value / maxVal) * 100.0);
            }

            // Verification Queue: Fetch unverified donors with full details for profile preview
            var dbUnverifiedDonors = await _context.Donors
                .Include(d => d.User)
                .Include(d => d.City)
                .Where(d => d.IsAdminVerified == false)
                .Select(d => new QueueItem
                {
                    DonorId = d.DonorId,
                    FullName = d.FullName,
                    Email = d.User.Email,
                    Phone = d.Phone,
                    Cnic = d.Cnic,
                    BloodGroup = d.BloodGroup,
                    Gender = d.Gender,
                    DateOfBirth = d.DateOfBirth,
                    CityName = d.City != null ? d.City.CityName : string.Empty,
                    Availability = d.Availability ?? "Unavailable",
                    DocumentType = "CNIC Verification",
                    SubmissionDate = d.CreatedAt.ToString("MMM dd, hh:mm tt")
                })
                .ToListAsync();

            VerificationQueue.Clear();
            VerificationQueue.AddRange(dbUnverifiedDonors);

            // Initialize Live Feed timeline with real events
            SystemLiveFeed.Clear();
            SystemLiveFeed.AddRange(_broadcastEvents);
        }
    }

    public class QueueItem
    {
        public int DonorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Cnic { get; set; } = string.Empty;
        public string BloodGroup { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string Availability { get; set; } = string.Empty;
        public string DocumentType { get; set; } = string.Empty;
        public string SubmissionDate { get; set; } = string.Empty;
    }

    public class TimelineEvent
    {
        public string Time { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ColorClass { get; set; } = "gray"; // red, blue, green
    }
}
