using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class ConfirmDeleteUser : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ConfirmDeleteUser> _logger;

        public ConfirmDeleteUser(UserManager<ApplicationUser> userManager, ApplicationDbContext context, ILogger<ConfirmDeleteUser> logger)
        {
            _userManager = userManager;
            _context = context;
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

        // public async Task<IActionResult> OnPostAsync(string id)
        // {
        //     Console.WriteLine("DeleteUserAsync called with id: " + id);
        //     var user = await _userManager.FindByIdAsync(id);
        //     if (user == null)
        //     {
        //         return NotFound();
        //     }

        //     // ВИДАЛЕННЯ STRIPE акаунту
        //     if (!string.IsNullOrEmpty(user.StripeCustomerId))
        //     {
        //         var service = new Stripe.CustomerService();
        //         await service.DeleteAsync(user.StripeCustomerId);
        //     }
        //     else
        //     {
        //         _logger.LogInformation("User doesn't have Stripe Customer ID");
        //     }

        //     // ВИДАЛЕННЯ ІНФОРМАЦІЇ ПРО СПОСОБИ ОПЛАТИ
        //     var userPayments = _context.PaymentMethods.Where(p => p.User.Id == id);
        //     _context.PaymentMethods.RemoveRange(userPayments);
        //     await _context.SaveChangesAsync();

        //     // ВИДАЛЕННЯ КОРИСТУВАЧА 
        //     var result = await _userManager.DeleteAsync(user);

        //     if (result.Succeeded)
        //     {
        //         _logger.LogInformation("User with ID {UserId} deleted.", id);
        //         return RedirectToPage("DashboardUsers");
        //     }
        //     else
        //     {
        //         // foreach (var error in result.Errors)
        //         // {
        //         //     ModelState.AddModelError(string.Empty, error.Description);
        //         // }
        //         foreach (var error in result.Errors)
        //         {
        //             Console.WriteLine($"Error deleting user {id}: Code={error.Code}, Desc={error.Description}");

        //             ModelState.AddModelError(string.Empty, error.Description);
        //         }
        //         return Page();
        //     }
        // }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            Console.WriteLine("DeleteUserAsync called with id: " + id);
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Спроба видалити Stripe акаунт 
            if (!string.IsNullOrEmpty(user.StripeCustomerId))
            {
                try
                {
                    var service = new Stripe.CustomerService();
                    await service.DeleteAsync(user.StripeCustomerId);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Stripe customer with ID {StripeId} could not be deleted: {Message}", user.StripeCustomerId, ex.Message);
                }
            }

            // Видалення інформації про способи оплати
            var userPayments = _context.PaymentMethods.Where(p => p.User.Id == id);
            _context.PaymentMethods.RemoveRange(userPayments);
            await _context.SaveChangesAsync();

            // Видалення користувача
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                _logger.LogInformation("User with ID {UserId} deleted.", id);
                return RedirectToPage("DashboardUsers");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error deleting user {id}: Code={error.Code}, Desc={error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }

    }
}