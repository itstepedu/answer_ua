using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class AddItem : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<AddItem> _logger;

        public AddItem(ShopDbContext shopDbContext, ILogger<AddItem> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public Product? ProductToAdd { get; set; } = new Product();
        public List<Brand> Brands { get; set; }
        public List<TargetCategories> Targets { get; set; }
        public List<ProductTypes> Types { get; set; }
        public List<Subcategories> Subcategories { get; set; }

        // ЗМІННА ДЛЯ РОБОТИ З КОЛЕКЦІЄЮ ФОТО
        [BindProperty]
        public string[] InputAdditionalURL { get; set; } = Array.Empty<string>();

        public void OnGet()
        {

            Brands = _shopDbContext.Brands.ToList();
            Targets = _shopDbContext.TargetCategories.ToList();
            Types = _shopDbContext.ProductTypes.ToList();
            Subcategories = _shopDbContext.Subcategories.ToList();
        }

        public IActionResult OnPost()
        {
            Brands = _shopDbContext.Brands.ToList();
            Targets = _shopDbContext.TargetCategories.ToList();
            Types = _shopDbContext.ProductTypes.ToList();
            Subcategories = _shopDbContext.Subcategories.ToList();

            ProductToAdd.Name = Request.Form["inputName"];

            ProductToAdd.BrandsId = int.Parse(Request.Form["inputBrand"]);
            ProductToAdd.TargetCategoriesId = int.Parse(Request.Form["inputTarget"]);
            ProductToAdd.ProductTypesId = int.Parse(Request.Form["inputType"]);
            ProductToAdd.SubcategoriesId = int.Parse(Request.Form["inputSubcategory"]);
            ProductToAdd.BrandsId = int.Parse(Request.Form["inputBrand"]);

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

            ProductToAdd.Size = string.Join("\n", formattedSize);
            ProductToAdd.Color = string.Join("\n", formattedColors);

            ProductToAdd.Stock = int.Parse(Request.Form["inputStock"]);
            ProductToAdd.Price = decimal.Parse(Request.Form["inputPrice"]);

            _shopDbContext.Add(ProductToAdd);
            var result = _shopDbContext.SaveChanges();

            // ДОДАВАННЯ ФОТО
            var ProductImagesToAdd = new List<ProductImages>();
            var order = 1;

            foreach (var url in InputAdditionalURL)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    ProductImagesToAdd.Add(new ProductImages
                    {
                        ProductId = ProductToAdd.Id,
                        ImageUrl = url,
                        Order = order
                    });
                    order++;
                }
            }

            _shopDbContext.ProductImages.AddRange(ProductImagesToAdd);
            _shopDbContext.SaveChanges();

            if (result > 0)
            {
                _logger.LogInformation("New product has been added.");
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