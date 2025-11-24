using System.ComponentModel.DataAnnotations;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AnswerUA.Views.Checkout
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public UserModel SingleUser { get; set; }

        [BindProperty]
        public AddressModel Address { get; set; }

        public class UserModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Display(Name = "Email")]
            public string Email { get; set; }

        }

        public class AddressModel
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
            SingleUser = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
            
            var address = user.Addresses?.FirstOrDefault();

            if (address != null)
            {
                Address = new AddressModel
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


        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }
    }
}
