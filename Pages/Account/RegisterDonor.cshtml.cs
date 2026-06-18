using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zariya.Pages.Account
{
    public class RegisterDonorModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public RegisterDonorModel(ZariyaDbContext context)
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
        public string? Bio { get; set; }

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

            // Check if email already exists
            var existingUser = await _context.Users.AnyAsync(u => u.Email == Email);
            if (existingUser)
            {
                ErrorMessage = "A user with this email address already exists.";
                return Page();
            }

            // Check if CNIC already exists
            var existingDonor = await _context.Donors.AnyAsync(d => d.Cnic == Cnic);
            if (existingDonor)
            {
                ErrorMessage = "A donor with this CNIC already exists.";
                return Page();
            }

            // Start transaction
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // 1. Create User
                    var user = new User
                    {
                        FullName = FullName,
                        Email = Email,
                        PasswordHash = "HASHED_" + Password,
                        Role = "donor",
                        IsEmailVerified = true,
                        IsActive = true,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // 2. Create Donor
                    var donor = new Zariya.Models.Donor
                    {
                        UserId = user.UserId,
                        FullName = FullName,
                        Cnic = Cnic,
                        DateOfBirth = DateOnly.FromDateTime(DateOfBirth),
                        Gender = Gender,
                        Phone = Phone,
                        CityId = CityId,
                        BloodGroup = BloodGroup,
                        DonationTypeId = 1, // Default Whole Blood
                        Availability = "Available",
                        IsAdminVerified = true,
                        TotalDonations = 0,
                        Bio = Bio,
                        ProfilePhoto = "/images/avatar_default.jpg",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _context.Donors.Add(donor);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    SuccessMessage = "Registration successful! You are now registered as an available blood donor.";
                    
                    // Reset fields
                    FullName = string.Empty;
                    Email = string.Empty;
                    Phone = string.Empty;
                    Cnic = string.Empty;
                    Bio = string.Empty;
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
