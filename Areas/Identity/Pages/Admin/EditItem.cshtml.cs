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
    public class EditItem : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<EditItem> _logger;

        public EditItem(ShopDbContext shopDbContext, ILogger<EditItem> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public Product? ProductToEdit { get; set; }
        public List<Brand> Brands { get; set; }
        public List<TargetCategories> Targets { get; set; }
        public List<ProductTypes> Types { get; set; }
        public List<Subcategories> Subcategories { get; set; }
        public List<ProductImages> ProductImages { get; set; }
        // public List<ProductColors> Colors { get; set; }
        // public List<ProductSizes> Sizes { get; set; }

        public IActionResult OnGet(string id)
        {
            ProductToEdit = _shopDbContext.Product
            .Include(p => p.Brands)
            .Include(p => p.Subcategories)
            .Include(p => p.TargetCategories)
            .Include(p => p.ProductTypes)
            .Include(p => p.Images)
            // .Include(p => p.Sizes)
            // .Include(p => p.Colors)
            .FirstOrDefault(p => p.Id.ToString() == id);

            Brands = _shopDbContext.Brands.ToList();
            Targets = _shopDbContext.TargetCategories.ToList();
            Types = _shopDbContext.ProductTypes.ToList();
            Subcategories = _shopDbContext.Subcategories.ToList();
            ProductImages = _shopDbContext.ProductImages.ToList();


            if (ProductToEdit == null)
            {
                return NotFound();
            }

            return Page();
        }

        public IActionResult OnPost(string id)
        {
            ProductToEdit = _shopDbContext.Product
            .Include(p => p.Brands)
            .Include(p => p.Subcategories)
            .Include(p => p.TargetCategories)
            .Include(p => p.ProductTypes)
            .FirstOrDefault(p => p.Id.ToString() == id);

            Brands = _shopDbContext.Brands.ToList();
            Targets = _shopDbContext.TargetCategories.ToList();
            Types = _shopDbContext.ProductTypes.ToList();
            Subcategories = _shopDbContext.Subcategories.ToList();

            if (ProductToEdit == null)
            {
                return NotFound();
            }

            ProductToEdit.Name = Request.Form["inputName"];

            ProductToEdit.BrandsId = int.Parse(Request.Form["inputBrand"]);
            ProductToEdit.TargetCategoriesId = int.Parse(Request.Form["inputTarget"]);
            ProductToEdit.ProductTypesId = int.Parse(Request.Form["inputType"]);
            ProductToEdit.SubcategoriesId = int.Parse(Request.Form["inputSubcategory"]);
            ProductToEdit.BrandsId = int.Parse(Request.Form["inputBrand"]);

            // ProductToEdit.Size = Request.Form["inputSize"];
            // ProductToEdit.Color = Request.Form["inputColor"];

            var size = Request.Form["inputSize"];
            var colors = Request.Form["inputColor"];

            var formattedSize = size.ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .ToList();

            var formattedColors = colors.ToString()
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(c => c.Trim())
            .ToList();

            ProductToEdit.Size = string.Join("\n", formattedSize);
            ProductToEdit.Color = string.Join("\n", formattedColors);

            ProductToEdit.Stock = int.Parse(Request.Form["inputStock"]);
            ProductToEdit.Price = decimal.Parse(Request.Form["inputPrice"]);

            ProductToEdit.ImageUrl = Request.Form["inputURL"];

            var result = _shopDbContext.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("Product with ID {ProductID} updated.", id);
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