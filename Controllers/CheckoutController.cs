using answer_ua.Data;
using AnswerUA.Data;
using AnswerUA.Models;
using AnswerUA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;

namespace AnswerUA.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _appDb;   // акаунти, адреси, картки
        private readonly ShopDbContext _shopDb;         // orders, orderitems, товари

        public CheckoutController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext appDb,
            ShopDbContext shopDb)
        {
            _userManager = userManager;
            _appDb = appDb;
            _shopDb = shopDb;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                       ?? new List<CartItem>();

            var addresses = _appDb.Addresses
                .Where(a => a.User.Id == user.Id)
                .ToList();

            var cards = _appDb.PaymentMethods
                .Where(c => c.User.Id == user.Id)
                .ToList();

            decimal subtotal = cart.Sum(c => c.Price * c.Quantity);
            decimal permanentDiscount = 0;
            decimal bonusDiscount = 0;

            if (user != null)
            {
                permanentDiscount = subtotal * (user.PermanentDiscount / 100m);
                bonusDiscount = Math.Min(user.AccumulatedPoints, subtotal - permanentDiscount);
            }

            decimal totalDiscount = permanentDiscount + bonusDiscount;
            decimal total = subtotal - totalDiscount;

            var model = new CheckoutViewModel
            {
                User = user,
                Cart = cart,
                Addresses = addresses,
                PaymentMethods = cards,
                Subtotal = subtotal,
                Discount = totalDiscount,
                Total = total
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> PlaceOrder(CheckoutOrderInput input)
        {
            var user = await _userManager.GetUserAsync(User);

            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");
            if (cart == null || cart.Count == 0)
                return RedirectToAction("Index", "Cart");

            // ОБЧИСЛЕННЯ TOTAL
            decimal total = cart.Sum(c => c.Price * c.Quantity);

            // НАРАХУВАННЯ ПОСТІЙНОЇ ЗНИЖКИ
            decimal permanentDiscountValue = 0;
            if (user != null && user.PermanentDiscount > 0)
            {
                permanentDiscountValue = total * (user.PermanentDiscount / 100m);
                total -= permanentDiscountValue;
            }

            {
                decimal bonusToUse = Math.Min(user.AccumulatedPoints, total);
                total -= bonusToUse;

                user.AccumulatedPoints -= bonusToUse;

                await _userManager.UpdateAsync(user);
            }

            // ---------- СТВОРЮЄМО ORDER В SHOPDbContext ----------
            var orders = new Orders
            {
                UserEmail = user.Email,
                OrderDate = DateTime.Now,
                Delivery = input.DeliveryMethod,
                Payment = input.PaymentMethod,
                DeliveryDate = DateTime.Now.AddDays(3),
                TotalAmount = total
            };

            _shopDb.Orders.Add(orders);
            _shopDb.SaveChanges();

            // ---------- ДОДАЄМО OrderItems ----------
            foreach (var item in cart)
            {
                var orderItem = new OrderItems
                {
                    OrdersId = orders.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Size = item.Size,
                    Colors = item.Color
                };

                _shopDb.OrderItems.Add(orderItem);

                var product = _shopDb.Product.FirstOrDefault(p => p.Id == item.ProductId);

                if (product != null)
                {
                    product.Stock -= item.Quantity;
                    if (product.Stock <= 0)
                    {
                        product.Stock = 0;
                    }
                }


            }

            _shopDb.SaveChanges();
            await UpdateUserLoyalty(user, orders.TotalAmount);

            // очищаємо корзину
            HttpContext.Session.Remove("cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }

        private async Task UpdateUserLoyalty(ApplicationUser user, decimal orderAmount)
        {
            // Нараховуємо бонуси (3%)
            user.AccumulatedPoints += Math.Round(orderAmount * 0.03m, 2);

            // Підрахунок загальної суми всіх замовлень
            var totalSpent = await _shopDb.Orders
                .Where(o => o.UserEmail == user.Email)
                .SumAsync(o => (double)o.TotalAmount);

            // Оновлення постійної знижки
            if (totalSpent >= 80000)
                user.PermanentDiscount = 10;
            else if (totalSpent >= 40000)
                user.PermanentDiscount = 7;
            else if (totalSpent >= 15000)
                user.PermanentDiscount = 5;
            else
                user.PermanentDiscount = 0;

            await _userManager.UpdateAsync(user);
        }
    }
}


