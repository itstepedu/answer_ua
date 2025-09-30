using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace answer_ua.Areas.Identity.Pages.Account.Manage
{
    public class BonusPointsDiscounts : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ILogger<BonusPointsDiscounts> _logger;

        public BonusPointsDiscounts(UserManager<ApplicationUser> userManager, ILogger<BonusPointsDiscounts> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [Display(Name = "Accumulated Points")]
        public decimal AccumulatedPoints { get; set; }

        [Display(Name = "Permanent Discount")]
        public decimal PermanentDiscount { get; set; }

        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                AccumulatedPoints = user.AccumulatedPoints;
                PermanentDiscount = user.PermanentDiscount;
            }
        }
    }
}