using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class DashboardOrders : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly ILogger<DashboardOrders> _logger;

        public DashboardOrders(ShopDbContext shopDbContext, ILogger<DashboardOrders> logger)
        {
            _shopDbContext = shopDbContext;
            _logger = logger;
        }

        public List<Orders> orders { get; set; }

        public void OnGet()
        {
            orders = _shopDbContext.Orders.ToList();
        }
    }
}