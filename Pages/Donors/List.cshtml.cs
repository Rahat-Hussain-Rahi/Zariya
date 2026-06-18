using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Zariya.Pages.Donors
{
    public class ListModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("/Donors/Search");
        }
    }
}
