using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Stripe.Climate;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class EditOrder : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<EditOrder> _logger;

        public EditOrder(ShopDbContext shopDbContext, ILogger<EditOrder> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public Orders? OrdersToEdit { get; set; }

        // ДОДАВАННЯ НОВОГО ТОВАРУ ДО ЧЕКУ
        public List<AnswerUA.Models.Product> Products { get; set; }

        public IActionResult OnGet(string id)
        {
            Products = _shopDbContext.Product.ToList();

            OrdersToEdit = _shopDbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Product)
            .FirstOrDefault(o => o.Id.ToString() == id);

            if (OrdersToEdit == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(string id)
        {
            OrdersToEdit = _shopDbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Product)
            .FirstOrDefault(o => o.Id.ToString() == id);

            if (OrdersToEdit == null)
            {
                return NotFound();
            }

            var dateValue = Request.Form["inputDateOfOrder"].ToString();
            if (!string.IsNullOrWhiteSpace(dateValue))
            {
                if (DateTime.TryParse(dateValue, out var parsedDate))
                {
                    OrdersToEdit.OrderDate = parsedDate;
                }
                else
                {
                    ModelState.AddModelError("OrderDate", "Invalid date format.");
                    return Page();
                }
            }

            OrdersToEdit.TotalAmount = decimal.Parse(Request.Form["inputTotalAmount"]);

            var result = _shopDbContext.SaveChanges();
            if (result > 0)
            {
                _logger.LogInformation("Order with ID {OrderID} updated.", id);
                return RedirectToPage("DashboardOrders");
            }
            else
            {
                _logger.LogInformation("No changes were saved");
                return Page();
            }
        }


        public IActionResult OnPostAddAdditionalItemToCheck(string id)
        {
            OrdersToEdit = _shopDbContext.Orders
            .FirstOrDefault(o => o.Id.ToString() == id);

            if (OrdersToEdit == null)
            {
                Console.WriteLine("THE ORDER IS NOT FOUND");
                return NotFound();
            }

            var productId = int.Parse(Request.Form["inputName"]);

            var product = _shopDbContext.Product.FirstOrDefault(p => p.Id == productId);
            if (product == null)
            {
                Console.WriteLine("THE PRODUCT IS NOT FOUND");
                return NotFound();
            }

            var quantity = int.Parse(Request.Form["inputQuantity"]);

            var existingItem = _shopDbContext.OrderItems
    .FirstOrDefault(oi => oi.ProductId == productId && oi.OrdersId == OrdersToEdit.Id);


            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var newOrderItem = new OrderItems
                {
                    OrdersId = OrdersToEdit.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Price = product.Price
                };
                _shopDbContext.OrderItems.Add(newOrderItem);

            }

            //         var totalAmount = _shopDbContext.OrderItems
            // .Where(oi => oi.OrdersId == OrdersToEdit.Id)
            // .ToList()
            // .Sum(oi => oi.Quantity * oi.Price);

            //         OrdersToEdit.TotalAmount += totalAmount;

            OrdersToEdit.TotalAmount += quantity * product.Price;

            _shopDbContext.SaveChanges();

            return RedirectToPage("EditOrder", new { id = id });
        }
    }
}