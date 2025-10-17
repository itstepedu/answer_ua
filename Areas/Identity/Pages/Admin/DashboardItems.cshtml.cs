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
    public class DashboardItems : PageModel
    {
        private readonly ILogger<DashboardItems> _logger;
        private readonly ShopDbContext _shopDbContext;

        public DashboardItems(ShopDbContext shopDbContext, ILogger<DashboardItems> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public List<Product> products { get; set; }

        public void OnGet()
        {

            products = _shopDbContext.Product
            .Include(p => p.Brands)
            .Include(p => p.Subcategories)
            .Include(p => p.TargetCategories)
            .Include(p => p.ProductTypes)
            .ToList(); 

        }
    }
}