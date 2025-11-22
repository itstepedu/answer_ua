using AnswerUA.Models;
using Microsoft.AspNetCore.Mvc;

public class WishController : Controller
{
    // ===========================
    // 1. Показати список вибраних
    // ===========================
    public IActionResult Index(string sort)
    {
        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();
      

        wish = sort switch
        {
            "cheap-first" => wish.OrderBy(x => x.Price).ToList(),
            "expensive-first" => wish.OrderByDescending(x => x.Price).ToList(),
            _ => wish
        };

        return View(wish);


    }
   

    // ===========================
    // 2. Додати у "Мій вибір"
    // ===========================
    [HttpPost]
    public IActionResult Add(int productId, string imageUrl, string name, string color, decimal price, string size, string slug)
    {
        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();

        // Перевірка чи товар уже є у вибраному
        var existing = wish.FirstOrDefault(c => c.ProductId == productId && c.Size == size);

        if (existing == null)
        {
            wish.Add(new WishItem
            {
                ProductId = productId,
                ImageUrl = imageUrl,
                Name = name,
                Color = color,
                Price = price,
                Size = size,
                Slug = slug
            });
        }

        HttpContext.Session.SetObjectAsJson("wish", wish);

        return RedirectToAction("Index");
    }

    // ===========================
    // 3. Видалити товар із вибраного
    // ===========================
    [HttpPost]
    public IActionResult Delete(int productId, string size)
    {
        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();

        var item = wish.FirstOrDefault(x => x.ProductId == productId && x.Size == size);

        if (item != null)
        {
            wish.Remove(item);
            HttpContext.Session.SetObjectAsJson("wish", wish);
        }

        return RedirectToAction("Index");
    }

    // ===========================
    // 4. Додати у кошик + видалити із WishList
    // ===========================
   
    public IActionResult AddToCart(int productId, string size)
    {
        if (!User.Identity.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login?returnUrl=/Wish");
        }
        // 1. Читаємо вибране
        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();

        var item = wish.FirstOrDefault(x => x.ProductId == productId && x.Size == size);

        if (item != null)
        {
            // 2. Додаємо в кошик
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                       ?? new List<CartItem>();

            var existingCartItem = cart.FirstOrDefault(c => c.ProductId == productId && c.Size == size);

            if (existingCartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = item.ProductId,
                    Name = item.Name,
                    Color = item.Color,
                    Price = item.Price,
                    Size = item.Size,
                    ImageUrl = item.ImageUrl,
                    Quantity = 1

                });
            }

            // зберігаємо оновлений кошик
            HttpContext.Session.SetObjectAsJson("cart", cart);

            // 3. Видаляємо з wish
            wish.Remove(item);
            HttpContext.Session.SetObjectAsJson("wish", wish);
        }

        return RedirectToAction("Index", "Cart");
    }
   

    /*[HttpPost]
    public IActionResult MoveSelectedToCart(List<string> selectedItems)
    {
        var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("cart")
                   ?? new List<CartItem>();

        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();

        if (selectedItems != null)
        {
            foreach (var value in selectedItems)
            {
                var parts = value.Split('|');
                int productId = int.Parse(parts[0]);
                string size = parts[1];

                var item = wish.FirstOrDefault(c => c.ProductId == productId && c.Size == size);
                if (item != null)
                {
                    cart.Add(new CartItem
                    {
                        ProductId = item.ProductId,
                        ImageUrl = item.ImageUrl,
                        Name = item.Name,
                        Color = item.Color,
                        Price = item.Price,
                        Size = item.Size,
                        Quantity = 1
                    });

                    wish.Remove(item);
                }
            }

            HttpContext.Session.SetObjectAsJson("cart", cart);
            HttpContext.Session.SetObjectAsJson("wish", wish);
        }

        return RedirectToAction("Index", "Cart");
    }
    
    [HttpPost]
    public IActionResult DeleteSelected(List<int> productIds, List<string> sizes)
    {
        var wish = HttpContext.Session.GetObjectFromJson<List<WishItem>>("wish")
                   ?? new List<WishItem>();

        for (int i = 0; i < productIds.Count; i++)
        {
            var productId = productIds[i];
            var size = sizes[i];

            var item = wish.FirstOrDefault(x => x.ProductId == productId && x.Size == size);

            if (item != null)
                wish.Remove(item);
        }

        HttpContext.Session.SetObjectAsJson("wish", wish);
        return RedirectToAction("Index");
    }*/

}

