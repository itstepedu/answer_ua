using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NuGet.Protocol.Plugins;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class EditUser : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<EditUser> _logger;

        public EditUser(UserManager<ApplicationUser> userManager, ILogger<EditUser> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public ApplicationUser? UserToEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            UserToEdit = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == id);

            if (UserToEdit == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            user.Email = Request.Form["inputEmail"];
            user.PhoneNumber = Request.Form["inputPhone"];
            user.FirstName = Request.Form["inputFirstName"];
            user.LastName = Request.Form["inputLastName"];
            user.DateOfBirth = DateTime.TryParse(Request.Form["inputDate"], out var date
            ) ? date : user.DateOfBirth;
            user.AccumulatedPoints = decimal.TryParse(Request.Form["inputPoints"], out var points
            ) ? points : user.AccumulatedPoints;
            user.PermanentDiscount = decimal.TryParse(Request.Form["inputDiscount"], out var discount
            ) ? discount : user.PermanentDiscount;

            
            var address = user.Addresses?.FirstOrDefault();
            if (address == null)
            {
                if (user.Addresses == null)
                {
                    user.Addresses = new List<Address>();
                }
                address = new Address();
                user.Addresses.Add(address);
            }

            address.StreetAddress = Request.Form["inputAddress"];
            address.SecondStreetAddress = Request.Form["inputAddress2"];
            address.City = Request.Form["inputCity"];
            address.Region = Request.Form["inputRegion"];
            address.PostalCode = Request.Form["inputZip"];


            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} updated.", id);
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