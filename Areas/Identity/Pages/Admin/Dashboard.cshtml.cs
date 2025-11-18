using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Admin
{
    public class Dashboard : PageModel
    {
        private readonly ILogger<DashboardUsers> _logger;

        public Dashboard(ILogger<DashboardUsers> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
        }
    }
}