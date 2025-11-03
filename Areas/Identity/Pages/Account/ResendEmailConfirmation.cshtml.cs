// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace AnswerUA.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

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
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        // public async Task<IActionResult> OnPostAsync()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return Page();
        //     }

        //     var user = await _userManager.FindByEmailAsync(Input.Email);
        //     if (user == null)
        //     {
        //         ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
        //         return Page();
        //     }

        //     Console.WriteLine("IM IN EMAIL CONFIRMATION");
        //     Console.WriteLine("Email: ", user.Email);
        //     Console.WriteLine("Username: ", user.UserName);


        //     var userId = await _userManager.GetUserIdAsync(user);
        //     var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //     code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        //     var callbackUrl = Url.Page(
        //         "/Account/ConfirmEmail",
        //         pageHandler: null,
        //         values: new { userId = userId, code = code },
        //         protocol: Request.Scheme);
        //     await _emailSender.SendEmailAsync(
        //         Input.Email,
        //         "Confirm your email",
        //         $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        //     ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
        //     return Page();
        // }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Console.WriteLine("Input.Email before lookup: ", Input.Email);

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // if (user == null)
            // {
            //     Console.WriteLine("No user found with email: ", Input.Email);
            //     ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
            //     return Page();
            // }

            if (user == null)
            {
                Console.WriteLine("User not found at all");
            }
            else
            {
                Console.WriteLine($"User object found: {user}");
                Console.WriteLine($"user.Email: {(user.Email == null ? "NULL" : user.Email)}");
                Console.WriteLine($"user.UserName: {(user.UserName == null ? "NULL" : user.UserName)}");
                Console.WriteLine($"user.NormalizedEmail: {(user.NormalizedEmail == null ? "NULL" : user.NormalizedEmail)}");
            }


            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            Console.WriteLine($"Sending email to: {Input.Email}");


            await _emailSender.SendEmailAsync(
                Input.Email,
                "Підтвердіть вашу електронну пошту",
                $"Будь ласка, підтвердіть ваш обліковий запис, <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>натиснувши тут</a>."
            );

            ModelState.AddModelError(string.Empty, "Лист для підтвердження надіслано. Будь ласка, перевірте свою електронну пошту.");
            return Page();
        }

    }
}
