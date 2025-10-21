using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Stripe.Climate;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class AddOrder : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<AddOrder> _logger;

        public AddOrder(UserManager<ApplicationUser> userManager, ShopDbContext shopDbContext, ILogger<AddOrder> logger)
        {
            _userManager = userManager;
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public List<ApplicationUser> Users { get; set; }
        public Orders newOrder { get; set; } = new Orders();

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }

        public IActionResult OnPost()
        {
            newOrder.UserEmail = Request.Form["inputEmail"];
            var dateValue = Request.Form["inputDateOfOrder"].ToString();

            if (!string.IsNullOrWhiteSpace(dateValue))
            {
                if (DateTime.TryParse(dateValue, out var parsedDate))
                {
                    newOrder.OrderDate = parsedDate;
                }
                else
                {
                    ModelState.AddModelError("OrderDate", "Invalid date format.");
                    return Page();
                }
            }

            newOrder.TotalAmount = 0;
            _shopDbContext.Add(newOrder);

            var result = _shopDbContext.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("A new order has been added");
                var newOrderId = newOrder.Id;

                return RedirectToPage("EditOrder", new { id = newOrderId });
            }
            else
            {
                _logger.LogInformation("No changes were saved");
                return Page();
            }

        }
    }
}