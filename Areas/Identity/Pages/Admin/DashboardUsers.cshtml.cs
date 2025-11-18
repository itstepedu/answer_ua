using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class DashboardUsers : PageModel
    {
        private readonly ILogger<DashboardUsers> _logger;
        private readonly UserManager<ApplicationUser> _userManager;


        public DashboardUsers(UserManager<ApplicationUser> userManager, ILogger<DashboardUsers> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public List<RoleManager<IdentityRole>> Roles { get; set; } = new List<RoleManager<IdentityRole>>();

        public async Task OnGetAsync()
        {
            Users = await _userManager.Users.ToListAsync();

            foreach (var user in Users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.Role = roles.FirstOrDefault() ?? "No Role";
            }

        }

        public async Task<IActionResult> ConfirmDeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectToPage("ConfirmDeleteUser", new { id = user.Id });
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return RedirectToPage("EditUser", new { id = user.Id });
        }


    }
}