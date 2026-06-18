using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Zariya.Data;
using Zariya.Models;

namespace Zariya.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ZariyaDbContext _context;

        public ContactModel(ZariyaDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string? Name { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        [BindProperty]
        public string? Subject { get; set; }
        [BindProperty]
        public string? Message { get; set; }

        public string? SuccessMessage { get; set; }

        public void OnGet()
        {
        }

        public async System.Threading.Tasks.Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid && !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(Subject) && !string.IsNullOrWhiteSpace(Message))
            {
                _context.ContactMessages.Add(new ContactMessage
                {
                    Name = Name,
                    Email = Email,
                    Subject = Subject,
                    Message = Message,
                    IsRead = false,
                    CreatedAt = DateTime.Now
                });
                await _context.SaveChangesAsync();

                SuccessMessage = "Thank you for contacting Zariya! We will get back to you shortly.";
                Name = string.Empty;
                Email = string.Empty;
                Subject = string.Empty;
                Message = string.Empty;
            }
            return Page();
        }
    }
}
