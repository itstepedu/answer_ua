// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace AnswerUA.Areas.Identity.Pages.Account.Manage
{
    public class EditProfile : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public EditProfile(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public InputAddressModel Address { get; set; }

        [BindProperty]
        public InputPasswordModel Password { get; set; }

        [BindProperty]
        public InputNewEmailModel NewEmail { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }
        }

        public class InputAddressModel
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

        public class InputPasswordModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current password")]
            public string OldPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New password")]
            public string NewPassword { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm new password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public class InputNewEmailModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "New email")]
            public string NewEmail { get; set; }
        }

        public bool HasLocalPassword { get; set; }
        public bool HasExternalLogin { get; set; }
        public string Email { get; set; }

        public List<PaymentMethod> savedCards { get; set; } = new();

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var email = await _userManager.GetEmailAsync(user);

            Username = userName;
            Email = email;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = user.DateOfBirth
            };

            var address = user.Addresses?.FirstOrDefault();
            Address = new InputAddressModel
            {
                StreetAddress = address?.StreetAddress ?? string.Empty,
                SecondStreetAddress = address?.SecondStreetAddress ?? string.Empty,
                City = address?.City ?? string.Empty,
                Region = address?.Region ?? string.Empty,
                PostalCode = address?.PostalCode ?? string.Empty
            };

            NewEmail = new InputNewEmailModel
            {
                NewEmail = email,
            };

            if (user.PaymentMethods != null && user.PaymentMethods.Any())
            {
                savedCards = user.PaymentMethods.OrderByDescending(p => p.IsDefault).ToList();
            }
            else
            {
                Console.WriteLine("No payment found for user.");
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.Users.
            Include(u => u.Addresses)
            .Include(u => u.PaymentMethods)
            .FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));
            HasLocalPassword = await _userManager.HasPasswordAsync(user);
            HasExternalLogin = (await _userManager.GetLoginsAsync(user)).Any();

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostChangeEmailAsync()
        {
            ModelState.Clear();

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var email = await _userManager.GetEmailAsync(user);
            if (NewEmail.NewEmail != email)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateChangeEmailTokenAsync(user, NewEmail.NewEmail);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmailChange",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, email = NewEmail.NewEmail, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    NewEmail.NewEmail,
                    "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                // StatusMessage = "Confirmation link to change email sent. Please check your email.";
                @TempData["Message"] = "Посилання для підтвердження зміни електронної пошти надіслано. Будь ласка, перевірте свою пошту.";
                return RedirectToPage();
            }

            @TempData["Message"] = "Ваша електронна пошта залишилася без змін.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostProfileAsync()
        {

            Console.WriteLine("IM IN PROFILE");
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                Console.WriteLine("USER IS NULL");
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Console.WriteLine("IM IN PROFILE TWO");


            // if (!ModelState.IsValid)
            // {
            //     foreach (var state in ModelState)
            //     {
            //         foreach (var error in state.Value.Errors)
            //         {
            //             Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
            //         }
            //     }


            //     await LoadAsync(user);
            //     return Page();
            // }
            ModelState.Clear();
            if (!TryValidateModel(Input, nameof(Input)))
            {
                foreach (var state in ModelState)
                {
                    foreach (var error in state.Value.Errors)
                    {
                        Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                    }
                }

                await LoadAsync(user);
                return Page();
            }

            Console.WriteLine("IM IN PROFILE THREE");

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
                Console.WriteLine("IM IN PROFILE THREE 2.0");
            }

            Console.WriteLine("IM IN PROFILE FOUR");


            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.DateOfBirth = Input.DateOfBirth;
            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostAddressAsync()
        {
            ModelState.Clear();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var address = user.Addresses?.FirstOrDefault() ?? new Address();

            address.StreetAddress = Address.StreetAddress;
            address.SecondStreetAddress = Address.SecondStreetAddress;
            address.City = Address.City;
            address.Region = Address.Region;
            address.Country = "Ukraine";
            address.PostalCode = Address.PostalCode;
            address.IsDefault = true;

            if (!user.Addresses.Contains(address))
                user.Addresses.Add(address);

            // if (address == null)
            // {
            //     user.Addresses = new List<AnswerUA.Models.Address>
            //     {
            //         new AnswerUA.Models.Address
            //         {
            //             StreetAddress = Address.StreetAddress,
            //             SecondStreetAddress = Address.SecondStreetAddress,
            //             City = Address.City,
            //             Region = Address.Region,
            //             Country = "Ukraine",
            //             PostalCode = Address.PostalCode,
            //             IsDefault = true
            //         }
            //     };
            // }
            // else
            // {
            //     address.StreetAddress = Input.StreetAddress;
            //     address.SecondStreetAddress = Input.SecondStreetAddress;
            //     address.City = Input.City;
            //     address.Region = Input.Region;
            //     address.PostalCode = Input.PostalCode;
            // }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred updating the user with ID '{user.Id}'.");
            }

            StatusMessage = "Your address has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPasswordAsync()
        {
            ModelState.Clear();

            Console.WriteLine("IM CHANGING PASSWORD");
            if (!ModelState.IsValid)
            {
                Console.WriteLine("IM NOT VALID");
                return Page();
            }


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Console.WriteLine("IM CHANGING PASSWORD TWO");
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, Password.OldPassword, Password.NewPassword);

            Console.WriteLine("IM CHANGING PASSWORD THREE");
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
            Console.WriteLine("OLD PASSWORD: " + Password.OldPassword);
            Console.WriteLine("PASSWORD HAS BEEN CHANGED TO: " + Password.NewPassword);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your password has been changed.";

            return RedirectToPage();
        }
    }
}
