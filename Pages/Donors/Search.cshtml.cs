using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zariya.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public SearchModel(ZariyaDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? BloodGroup { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CityId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? CityName { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool AvailableNow { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public bool VerifiedOnly { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public string? SortBy { get; set; } = "Closest First";

        public List<Zariya.Models.Donor> DonorsList { get; set; } = new List<Zariya.Models.Donor>();
        public List<City> CitiesList { get; set; } = new List<City>();
        public List<string> BloodGroups { get; set; } = new List<string> { "A+", "A-", "B+", "B-", "O+", "O-", "AB+", "AB-" };
        public bool IsLoggedIn { get; set; }

        public async Task OnGetAsync()
        {
            IsLoggedIn = HttpContext.Session.GetString("UserEmail") != null;

            // Populate dropdowns
            CitiesList = await _context.Cities.Where(c => c.IsActive == true).OrderBy(c => c.CityName).ToListAsync();

            // Build query
            var query = _context.Donors
                .Include(d => d.City)
                .Include(d => d.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(BloodGroup))
            {
                query = query.Where(d => d.BloodGroup == BloodGroup);
            }

            if (CityId.HasValue)
            {
                query = query.Where(d => d.CityId == CityId.Value);
            }
            else if (!string.IsNullOrEmpty(CityName))
            {
                query = query.Where(d => d.City != null && d.City.CityName.Contains(CityName));
            }

            if (AvailableNow)
            {
                query = query.Where(d => d.Availability == "Available");
            }

            if (VerifiedOnly)
            {
                query = query.Where(d => d.IsAdminVerified == true);
            }

            // Order query
            if (SortBy == "Closest First")
            {
                query = query.OrderBy(d => d.Availability == "Available" ? 0 : 1)
                             .ThenByDescending(d => d.IsAdminVerified)
                             .ThenBy(d => d.FullName);
            }
            else
            {
                query = query.OrderBy(d => d.FullName);
            }

            DonorsList = await query.ToListAsync();
        }
    }
}
