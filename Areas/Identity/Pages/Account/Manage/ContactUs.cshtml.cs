using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using AnswerUA.Services;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class ContactUs : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly EmailSender _emailSender;

        private readonly ILogger<ContactUs> _logger;

        public ContactUs(UserManager<ApplicationUser> userManager, EmailSender emailSender, ILogger<ContactUs> logger)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public string Email { get; set; }
        [Display(Name = "First name")]

        [BindProperty]
        public string FirstName { get; set; }

        public class InputModel
        {
            [Display(Name = "Subject")]
            public string Subject { get; set; }

            [Display(Name = "Message")]
            public string Message { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var email = await _userManager.GetEmailAsync(user);
            var firstName = user.FirstName;

            Email = email;
            FirstName = firstName;

            Input = new InputModel
            {
                Subject = string.Empty,
                Message = string.Empty
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
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

            var user = await _userManager.GetUserAsync(User);
            string userEmail = "yaroslavcharlies@gmail.com";
            string subject = Input.Subject;
            string messageBody = $"From: {user.FirstName} ({user.Email})<br/><br/>{Input.Message}";


            await _emailSender.SendEmailWithReply(userEmail, subject, messageBody, user.Email, user.FirstName);
            StatusMessage = "Your message has been successfully sent. We'll get back to you soon.";
            return RedirectToPage();
        }



    }
}