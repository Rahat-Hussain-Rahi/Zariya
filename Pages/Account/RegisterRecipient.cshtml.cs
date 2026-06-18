using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;

namespace Zariya.Pages.Account
{
    public class RegisterRecipientModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public RegisterRecipientModel(ZariyaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string FullName { get; set; } = string.Empty;

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public string Phone { get; set; } = string.Empty;

        [BindProperty]
        public string Cnic { get; set; } = string.Empty;

        [BindProperty]
        public DateTime DateOfBirth { get; set; } = DateTime.Today.AddYears(-20);

        [BindProperty]
        public string Gender { get; set; } = "Male";

        [BindProperty]
        public int CityId { get; set; }

        [BindProperty]
        public string BloodGroup { get; set; } = "A+";

        [BindProperty]
        public string? MedicalCondition { get; set; }

        [BindProperty]
        public string UrgencyLevel { get; set; } = "normal";

        public List<City> CitiesList { get; set; } = new List<City>();
        public List<string> BloodGroups { get; set; } = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };

        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCitiesAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCitiesAsync();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var existingUser = await _context.Users.AnyAsync(u => u.Email == Email);
            if (existingUser)
            {
                ErrorMessage = "A user with this email address already exists.";
                return Page();
            }

            var existingPatient = await _context.Patients.AnyAsync(p => p.Cnic == Cnic);
            if (existingPatient)
            {
                ErrorMessage = "A recipient with this CNIC already exists.";
                return Page();
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var user = new User
                    {
                        FullName = FullName,
                        Email = Email,
                        PasswordHash = "HASHED_" + Password,
                        Role = "patient",
                        IsEmailVerified = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    var patient = new Patient
                    {
                        UserId = user.UserId,
                        FullName = FullName,
                        Cnic = Cnic,
                        DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                        Gender = Gender,
                        Phone = Phone,
                        CityId = CityId,
                        BloodGroup = BloodGroup,
                        MedicalCondition = MedicalCondition,
                        UrgencyLevel = UrgencyLevel,
                        ProfilePhoto = "/images/avatar_default.jpg",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Patients.Add(patient);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    SuccessMessage = "Registration successful! You can now log in and request blood donations.";
                    FullName = string.Empty;
                    Email = string.Empty;
                    Phone = string.Empty;
                    Cnic = string.Empty;
                    MedicalCondition = string.Empty;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ErrorMessage = $"An error occurred during registration: {ex.Message}";
                }
            }

            return Page();
        }

        private async Task LoadCitiesAsync()
        {
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();
        }
    }
}
