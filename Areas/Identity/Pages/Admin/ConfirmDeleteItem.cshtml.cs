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
    public class ConfirmDeleteItem : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<ConfirmDeleteItem> _logger;

        public ConfirmDeleteItem(ShopDbContext shopDbContext, ILogger<ConfirmDeleteItem> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        [BindProperty]
        public Product? ProductToDelete { get; set; }

        public IActionResult OnGet(string id)
        {
            ProductToDelete = _shopDbContext.Product.
            Include(p => p.Brands)
            .FirstOrDefault(p => p.Id.ToString() == id);

            if (ProductToDelete == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(string id)
        {
            ProductToDelete = _shopDbContext.Product
            // .Include(p => p.Sizes)
            // .Include(p => p.Colors)
            .FirstOrDefault(p => p.Id.ToString() == id);



            if (ProductToDelete == null)
            {
                return NotFound();
            }

            var orderToDelete = _shopDbContext.OrderItems.Where(o => o.ProductId.ToString() == id);
            var ColorItemToDelete = _shopDbContext.ProductColors.Where(o => o.ProductId.ToString() == id);
            var SizeItemToDelete = _shopDbContext.ProductSizes.Where(o => o.ProductId.ToString() == id);

            // var orderIds = _shopDbContext.OrderItems.Where(o => o.ProductId.ToString() == id)
            // .Select(o => o.OrdersId).ToList();
            // var ordersToChange = _shopDbContext.Orders.Where(order => orderIds.Contains(order.Id));

            foreach (var item in orderToDelete)
            {
                var order = _shopDbContext.Orders.FirstOrDefault(o => o.Id == item.OrdersId);
                if (order != null)
                {
                    order.TotalAmount -= item.Price * item.Quantity;

                    if (order.TotalAmount < 0)
                    {
                        order.TotalAmount = 0;
                    }
                }
            }
            
            _shopDbContext.OrderItems.RemoveRange(orderToDelete);
            _shopDbContext.ProductColors.RemoveRange(ColorItemToDelete);
            _shopDbContext.ProductSizes.RemoveRange(SizeItemToDelete);


            _shopDbContext.Product.Remove(ProductToDelete);
            var result = _shopDbContext.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("Product with ID {ProductID} deleted.", id);
                return RedirectToPage("DashboardItems");
            }
            else
            {
                _logger.LogInformation("No changes were saved");
                return Page();
            }
        }
    }
}