using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class ConfirmDeleteOrder : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<ConfirmDeleteOrder> _logger;

        public ConfirmDeleteOrder(ShopDbContext shopDbContext, ILogger<ConfirmDeleteOrder> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        [BindProperty]
        public Orders? OrderToDelete { get; set; }

        public IActionResult OnGet(string id)
        {
            OrderToDelete = _shopDbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Product)
            .FirstOrDefault(o => o.Id.ToString() == id);


            if (OrderToDelete == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(string id)
        {
            OrderToDelete = _shopDbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Product)
            .FirstOrDefault(o => o.Id.ToString() == id);

            if (OrderToDelete == null)
            {
                return NotFound();
            }

            _shopDbContext.Orders.Remove(OrderToDelete);
            var result = _shopDbContext.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("Order with ID {OrderID} deleted.", id);
                return RedirectToPage("DashboardOrders");
            }
            else
            {
                _logger.LogInformation("No changes were saved");
                return Page();
            }

        }
    }
}