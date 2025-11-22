using answer_ua.Data;
using AnswerUA.Data;
using AnswerUA.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("product")]
public class ProductController : Controller
{
    private readonly ShopDbContext _db;

    public ProductController(ShopDbContext db)
    {
        _db = db;
    }

    [HttpGet("{id:int}/{slug?}")]
    public async Task<IActionResult> Details(int id, string slug)
    {
        var product = await _db.Product
            .Include(p => p.Brands)
            .Include(p => p.Subcategories)
            .Include(p => p.Images)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (product == null)
            return NotFound();

        var vm = new ProductDetailsVm
        {
            Id = product.Id,
            Name = product.Name,
            Brands = product.Brands?.Name,
            Price = product.Price,
            Color = product.Color,
            ImageUrl = product.ImageUrl,
           

            Size = await _db.ProductSizes
                .Where(s => s.ProductId == id)
                .Select(s => s.Size)
                .ToListAsync(),
            Images = product.Images.OrderBy(i => i.Order).Select(i => i.ImageUrl).ToList(),
            TargetSlug = AnswerUA.Utils.Ids.SlugFromTargetId(product.TargetCategoriesId),
            ProductTypesSlug = AnswerUA.Utils.Ids.SlugFromTypeId(product.ProductTypesId),
            SubcategoriesName = product.Subcategories?.Name
        };

        return View("Details", vm);
    }
}
