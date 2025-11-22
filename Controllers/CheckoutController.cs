using answer_ua.Data;
using AnswerUA.Data;
using AnswerUA.Models;
using AnswerUA.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

            var model = new CheckoutViewModel
            {
                User = user,
                Cart = cart,
                Addresses = addresses,
                PaymentMethods = cards
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

            // ---------- СТВОРЮЄМО ORDER В SHOPDbContext ----------
            var orders = new Orders
            {
                UserEmail = user.Email,
                OrderDate = DateTime.Now,
                Delivery = input.DeliveryMethod,
                Payment = input.PaymentMethod,
                DeliveryDate = DateTime.Now.AddDays(3),
                TotalAmount = cart.Sum(c => c.Price * c.Quantity)
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
            }

            _shopDb.SaveChanges();

            // очищаємо корзину
            HttpContext.Session.Remove("cart");

            return RedirectToAction("Success");
        }

        public IActionResult Success()
        {
            return View();
        }
    }
}


