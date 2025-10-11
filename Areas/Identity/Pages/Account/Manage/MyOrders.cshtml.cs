using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class MyOrders : PageModel
    {
        private readonly ILogger<MyOrders> _logger;

        public MyOrders(ILogger<MyOrders> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}