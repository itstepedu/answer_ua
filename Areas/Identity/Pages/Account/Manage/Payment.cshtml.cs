using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using Stripe;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class Payment : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Payment> _logger;


        public Payment(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<Payment> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public List<AnswerUA.Models.PaymentMethod> savedCards { get; set; } = new();

        public string? CardHolderName { get; set; }

        public string StripeCustomerId { get; set; }
        [BindProperty]
        public string PaymentMethodId { get; set; }

        public async Task LoadAsync(ApplicationUser user)
        {
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
            ViewData["StripePublishableKey"] = _configuration["Stripe:PublishableKey"];

            var user = await _userManager.Users.Include(u => u.PaymentMethods).FirstOrDefaultAsync(u => u.Id == _userManager.GetUserId(User));

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (string.IsNullOrEmpty(user.StripeCustomerId))
            {
                Console.WriteLine(user.FirstName);
                Console.WriteLine(user.LastName);

                var options = new Stripe.CustomerCreateOptions
                {
                    Email = user.Email,
                    Metadata = new Dictionary<string, string>
                    {
                        {"first_name", user.FirstName},
                        {"last_name", user.LastName}
                    }
                    // Name = $"{user.FirstName} {user.LastName}"
                };

                var service = new Stripe.CustomerService();
                var customer = await service.CreateAsync(options);

                user.StripeCustomerId = customer.Id;
                await _userManager.UpdateAsync(user);
            }

            StripeCustomerId = user.StripeCustomerId;

            await LoadAsync(user);
            return Page();
        }

        // public async Task<IActionResult> OnPostAsync()
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return Page();
        //     }

        //     var user = await _userManager.GetUserAsync(User);
        //     if (user == null)
        //     {
        //         return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
        //     }


        //     return RedirectToPage();
        // }
    }
}