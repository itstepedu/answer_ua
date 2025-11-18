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
    public class AddUser : PageModel
    {
        private readonly ILogger<AddUser> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AddUser(UserManager<ApplicationUser> userManager, ILogger<AddUser> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public ApplicationUser newUser { get; set; } = new ApplicationUser();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            newUser.Email = Request.Form["inputEmail"];
            newUser.UserName = Request.Form["inputEmail"];
            newUser.PhoneNumber = Request.Form["inputPhone"];

            newUser.FirstName = Request.Form["inputFirstName"];
            newUser.LastName = Request.Form["inputLastName"];
            newUser.DateOfBirth = DateTime.TryParse(Request.Form["inputDate"], out var date
            ) ? date : newUser.DateOfBirth;
            newUser.AccumulatedPoints = decimal.TryParse(Request.Form["inputPoints"], out var points
            ) ? points : newUser.AccumulatedPoints;
            newUser.PermanentDiscount = decimal.TryParse(Request.Form["inputDiscount"], out var discount
            ) ? discount : newUser.PermanentDiscount;


            newUser.Addresses = new Address[]
            {
                new Address
                {
                    StreetAddress = Request.Form["inputAddress"],
                    SecondStreetAddress = Request.Form["inputAddress2"],
                    City = Request.Form["inputCity"],
                    Region = Request.Form["inputRegion"],
                    PostalCode = Request.Form["inputZip"],
                    Country = "Ukraine",
                    IsDefault = true
                }
            }.ToList();

            Console.WriteLine(Request.Form["inputEmail"]);
            Console.WriteLine(Request.Form["inputPhone"]);
            Console.WriteLine(Request.Form["inputFirstName"]);
            Console.WriteLine(Request.Form["inputLastName"]);
            Console.WriteLine(Request.Form["inputDate"]);
            Console.WriteLine(Request.Form["inputPoints"]);
            Console.WriteLine(Request.Form["inputDiscount"]);


            var result = await _userManager.CreateAsync(newUser, "12345Abc@");

            if (result.Succeeded)
            {
                return RedirectToPage("Dashboard");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"{error.Code}: {error.Description}");


                }
                return Page();
            }
        }
    }
}