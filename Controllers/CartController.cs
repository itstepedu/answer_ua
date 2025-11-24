// using AnswerUA.Models;
// using Microsoft.AspNetCore.Mvc;

// public class CartController : Controller
// {
//     public IActionResult Index()
//     {
//         var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
//                    ?? new List<CartItem>();

//         return View(cart);
//     }

//     [HttpPost]
//     public IActionResult AddToCart(int productId, string imageurl, string name, string color, decimal price, string size)
//     {
//         var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
//                    ?? new List<CartItem>();

//         var existing = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);

//         if (existing != null)
//         {
//             existing.Quantity++;
//         }
//         else
//         {
//             cart.Add(new CartItem
//             {
//                 ProductId = productId,
//                 ImageUrl = imageurl,
//                 Name = name,
//                 Color = color,
//                 Price = price,
//                 Size = size,
//                 Quantity = 1
//             });
//         }

//         HttpContext.Session.SetObjectAsJson("cart", cart);

//         return RedirectToAction("Index");
//     }

//     [HttpPost]
//     public IActionResult UpdateQuantity(int productId, string size, int quantity)
//     {
//         var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
//                    ?? new List<CartItem>();

//         var item = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);

//         if (item != null)
//         {
//             item.Quantity = quantity;
//         }

//         HttpContext.Session.SetObjectAsJson("cart", cart);
//         return RedirectToAction("Index");
//     }
//     [HttpPost]
//     public IActionResult Delete(int productId, string size)
//     {
//         // 1. Читаємо корзину
//         var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

//         if (cart == null)
//             return RedirectToAction("Index");

//         // 2. Знаходимо товар
//         var item = cart.FirstOrDefault(x => x.ProductId == productId && x.Size == size);

//         // 3. Видаляємо товар
//         if (item != null)
//         {
//             cart.Remove(item);
//             HttpContext.Session.SetObjectAsJson("cart", cart);
//         }

//         return RedirectToAction("Index");
//     }

// }
using System.Threading.Tasks;
using AnswerUA.Models;
using AnswerUA.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class CartController : Controller
{
    // public IActionResult Index()
    // {
    //     var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
    //                ?? new List<CartItem>();

    //     return View(cart);
    // }

    private readonly UserManager<ApplicationUser> _userManager;


    public CartController(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                   ?? new List<CartItem>();

        decimal subtotal = cart.Sum(c => c.Price * c.Quantity);

        decimal permanentDiscount = 0;
        decimal bonusDiscount = 0;

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                permanentDiscount = subtotal * (user.PermanentDiscount / 100m);
                bonusDiscount = Math.Min(user.AccumulatedPoints, subtotal - permanentDiscount);
            }
        }

        decimal totalDiscount = permanentDiscount + bonusDiscount;
        decimal total = subtotal - totalDiscount;

        var model = new CartViewModel
        {
            Items = cart,
            Subtotal = subtotal,
            Discount = totalDiscount,
            Total = total
        };

        return View(model);
    }


    [HttpPost]
    public IActionResult AddToCart(int productId, string imageurl, string name, string color, decimal price, string size)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect($"/Identity/Account/Login?returnUrl=/Product/Details/{productId}");
        }
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                   ?? new List<CartItem>();

        var existing = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size && c.Color == color);

        if (existing != null)
        {
            existing.Quantity++;
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ImageUrl = imageurl,
                Name = name,
                Color = color,
                Price = price,
                Size = size,
                Quantity = 1
            });
        }

        HttpContext.Session.SetObjectAsJson("cart", cart);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult UpdateQuantity(int productId, string size, int quantity)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                   ?? new List<CartItem>();

        var item = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);

        if (item != null)
        {
            item.Quantity = quantity;
        }

        HttpContext.Session.SetObjectAsJson("cart", cart);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public IActionResult Delete(int productId, string size)
    {
        // 1. Читаємо корзину
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart");

        if (cart == null)
            return RedirectToAction("Index");

        // 2. Знаходимо товар
        var item = cart.FirstOrDefault(x => x.ProductId == productId && x.Size == size);

        // 3. Видаляємо товар
        if (item != null)
        {
            cart.Remove(item);
            HttpContext.Session.SetObjectAsJson("cart", cart);
        }

        return RedirectToAction("Index");
    }

}
