using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Zariya.Data;
using Zariya.Models;

namespace Zariya.Pages.Requests
{
    public class DetailsModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public DetailsModel(ZariyaDbContext context)
        {
            _context = context;
        }

        public Request? BloodRequest { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (HttpContext.Session.GetString("UserEmail") == null)
            {
                return RedirectToPage("/Account/Login", new { returnUrl = Url.Page("/Requests/Details", new { id }) });
            }

            BloodRequest = await _context.Requests
                .Include(r => r.Patient)
                .ThenInclude(p => p.City)
                .Include(r => r.Donor)
                .FirstOrDefaultAsync(r => r.RequestId == id);

            if (BloodRequest == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
