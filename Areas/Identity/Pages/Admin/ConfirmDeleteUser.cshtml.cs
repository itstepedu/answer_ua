using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class ConfirmDeleteUser : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ConfirmDeleteUser> _logger;

        public ConfirmDeleteUser(UserManager<ApplicationUser> userManager, ILogger<ConfirmDeleteUser> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public ApplicationUser? UserToDelete { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UserToDelete = await _userManager.FindByIdAsync(id);
            if (UserToDelete == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            Console.WriteLine("DeleteUserAsync called with id: " + id);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} deleted.", id);
                return RedirectToPage("Dashboard");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}