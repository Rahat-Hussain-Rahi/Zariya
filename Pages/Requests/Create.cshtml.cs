using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;

namespace Zariya.Pages.Requests;

public class CreateModel : PageModel
{
    private readonly ZariyaDbContext _context;

    public CreateModel(ZariyaDbContext context)
    {
        _context = context;
    }

    public List<City> CitiesList { get; set; } = new();
    public List<string> BloodGroups { get; set; } = new() { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

    [BindProperty]
    public string BloodGroupNeeded { get; set; } = string.Empty;

    [BindProperty]
    public string Urgency { get; set; } = "normal";

    [BindProperty]
    public string? Message { get; set; }

    [BindProperty]
    public string? PatientName { get; set; }

    [BindProperty]
    public string? PatientPhone { get; set; }

    [BindProperty]
    public int? PatientCityId { get; set; }

    [BindProperty]
    public string? PatientMedicalCondition { get; set; }

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsPatient { get; set; }
    public Patient? CurrentPatient { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        var role = HttpContext.Session.GetString("UserRole");
        IsPatient = role == "patient" || role == "recipient";

        if (IsPatient)
        {
            CurrentPatient = await _context.Patients
                .Include(p => p.City)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        if (userId == null) return RedirectToPage("/Account/Login");

        var role = HttpContext.Session.GetString("UserRole");
        IsPatient = role == "patient" || role == "recipient";

        CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

        if (string.IsNullOrWhiteSpace(BloodGroupNeeded))
        {
            ErrorMessage = "Please select a blood group.";
            return Page();
        }

        try
        {
            Patient patient;

            if (IsPatient)
            {
                CurrentPatient = await _context.Patients
                    .Include(p => p.City)
                    .FirstOrDefaultAsync(p => p.UserId == userId);

                if (CurrentPatient == null)
                {
                    ErrorMessage = "Patient profile not found.";
                    return Page();
                }

                patient = CurrentPatient;
            }
            else
            {
                if (string.IsNullOrWhiteSpace(PatientName))
                {
                    ErrorMessage = "Please enter the patient's full name.";
                    return Page();
                }

                var tempEmail = "tmp_" + Guid.NewGuid().ToString("N")[..8] + "@zariya.local";
                var tempUser = new User
                {
                    FullName = PatientName,
                    Email = tempEmail,
                    PasswordHash = "HASHED_temporary",
                    Role = "patient",
                    IsEmailVerified = false,
                    IsActive = true,
                    FailedAttempts = 0,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                _context.Users.Add(tempUser);
                await _context.SaveChangesAsync();

                patient = new Patient
                {
                    UserId = tempUser.UserId,
                    FullName = PatientName,
                    Phone = PatientPhone ?? "",
                    CityId = PatientCityId,
                    BloodGroup = BloodGroupNeeded,
                    MedicalCondition = PatientMedicalCondition,
                    UrgencyLevel = Urgency,
                    Cnic = "TEMP-" + Guid.NewGuid().ToString("N")[..10],
                    DateOfBirth = DateOnly.FromDateTime(DateTime.Now.AddYears(-30)),
                    Gender = "Not specified",
                    ProfilePhoto = "/images/avatar_default.jpg",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }

            var request = new Request
            {
                PatientId = patient.PatientId,
                BloodGroupNeeded = BloodGroupNeeded,
                Urgency = Urgency,
                Message = string.IsNullOrEmpty(Message) ? "Urgent blood donation required." : Message,
                Status = "pending",
                IsFulfilled = false,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Requests.Add(request);
            await _context.SaveChangesAsync();

            var matchingDonors = await _context.Donors
                .Where(d => d.BloodGroup == BloodGroupNeeded
                    && d.CityId == patient.CityId
                    && d.IsAdminVerified
                    && d.Availability == "Available")
                .CountAsync();

            var searchLog = new SearchLog
            {
                PatientId = patient.PatientId,
                SearchCityId = patient.CityId,
                BloodGroupFilter = BloodGroupNeeded,
                AvailabilityFilter = "Available",
                ResultsCount = (short)matchingDonors,
                CreatedAt = DateTime.Now
            };
            _context.SearchLogs.Add(searchLog);
            await _context.SaveChangesAsync();

            SuccessMessage = $"Blood request submitted successfully! {matchingDonors} matching donor(s) in your city have been notified.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error creating request: {ex.Message}";
        }

        return Page();
    }
}
