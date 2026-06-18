using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;

namespace Zariya.Pages.Donors
{
    public class DetailsModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public DetailsModel(ZariyaDbContext context)
        {
            _context = context;
        }

        public Zariya.Models.Donor? Donor { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Donors/Details", new { id }) });
            }

            Donor = await _context.Donors
                .Include(d => d.User)
                .Include(d => d.City)
                .FirstOrDefaultAsync(d => d.DonorId == id && d.IsAdminVerified);

            if (Donor == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
