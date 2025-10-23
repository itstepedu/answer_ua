using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using answer_ua.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class MyOrders : PageModel
    {
        private readonly ShopDbContext _shopDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<MyOrders> _logger;

        public MyOrders(ShopDbContext shopDbContext, UserManager<ApplicationUser> userManager, ILogger<MyOrders> logger)
        {
            _shopDbContext = shopDbContext;
            _userManager = userManager;
            _logger = logger;
        }

        public List<Orders> UserOrders { get; set; }
        public ApplicationUser CurrentUser { get; set; }

        public async Task OnGet()
        {
            CurrentUser = await _userManager.GetUserAsync(User);
            var userEmail = await _userManager.GetEmailAsync(CurrentUser);

            UserOrders = await _shopDbContext.Orders
            .Include(o => o.OrderItems)
            .ThenInclude(o => o.Product)
            .Where(uo => uo.UserEmail == userEmail)
            .ToListAsync();
        }
    }
}