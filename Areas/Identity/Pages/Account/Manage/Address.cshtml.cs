using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class Address : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Address> _logger;

        public Address(UserManager<ApplicationUser> userManager, ILogger<Address> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Display(Name = "Address")]
            public string? StreetAddress { get; set; }
            [Display(Name = "Address 2")]
            public string? SecondStreetAddress { get; set; }
            [Display(Name = "City")]
            public string? City { get; set; }
            [Display(Name = "Region")]
            public string? Region { get; set; }
            [Display(Name = "Zip Code")]
            public string? PostalCode { get; set; }
        }

        public async Task LoadAsync(ApplicationUser user)
        {
            var address = user.Addresses?.FirstOrDefault();

            if (address != null)
            {
                Input = new InputModel
                {
                    StreetAddress = address.StreetAddress,
                    SecondStreetAddress = address.SecondStreetAddress,
                    City = address.City,
                    Region = address.Region,
                    PostalCode = address.PostalCode
                };
            }
            else
            {
                Console.WriteLine("No address found for user.");
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var address = user.Addresses?.FirstOrDefault();

            if (address == null)
            {
                user.Addresses = new List<AnswerUA.Models.Address>
                {
                    new AnswerUA.Models.Address
                    {
                        StreetAddress = Input.StreetAddress,
                        SecondStreetAddress = Input.SecondStreetAddress,
                        City = Input.City,
                        Region = Input.Region,
                        Country = "Ukraine",
                        PostalCode = Input.PostalCode,
                        IsDefault = true
                    }
                };
            }
            else
            {
                address.StreetAddress = Input.StreetAddress;
                address.SecondStreetAddress = Input.SecondStreetAddress;
                address.City = Input.City;
                address.Region = Input.Region;
                address.PostalCode = Input.PostalCode;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred updating the user with ID '{user.Id}'.");
            }

            StatusMessage = "Your address has been updated";
            return RedirectToPage();
        }
    }

}