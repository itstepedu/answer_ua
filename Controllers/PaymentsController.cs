using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnswerUA.Data;
using AnswerUA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe.Climate;

namespace answer_ua.Controllers
{
    [ApiController]
    [Route("api/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;


        public PaymentsController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("create-setup-intent")]
        public async Task<IActionResult> CreateSetupIntent([FromBody] CreateSetupRequest request)
        {
            var options = new Stripe.SetupIntentCreateOptions
            {
                Customer = request.StripeCustomerId
            };

            var service = new Stripe.SetupIntentService();
            var intent = await service.CreateAsync(options);

            return Ok(new
            {
                ClientSecret = intent.ClientSecret
            });
        }

        [HttpPost("save-payment-method")]
        public async Task<IActionResult> SavePaymentMethod([FromBody] SavePaymentMethodRequest request)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == request.StripeCustomerId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var service = new Stripe.PaymentMethodService();
            var paymentMethod = await service.GetAsync(request.PaymentMethodId);

            var existing = await _context.PaymentMethods.FirstOrDefaultAsync(p => p.User.Id == user.Id
            && p.Last4 == paymentMethod.Card.Last4
            && p.ExpMonth == paymentMethod.Card.ExpMonth
            && p.ExpYear == paymentMethod.Card.ExpYear);

            if (existing == null)
            {
                var payment = new AnswerUA.Models.PaymentMethod
                {
                    User = user,
                    Brand = paymentMethod.Card.Brand,
                    Last4 = paymentMethod.Card.Last4,
                    ExpMonth = paymentMethod.Card.ExpMonth,
                    ExpYear = paymentMethod.Card.ExpYear,
                    CardHolderName = paymentMethod.BillingDetails?.Name,
                    IsDefault = true,
                    PaymentMethodId = request.PaymentMethodId

                };

                _context.PaymentMethods.Add(payment);
                await _context.SaveChangesAsync();
            }
            else
            {
                existing.IsDefault = true;
                existing.CardHolderName = paymentMethod.BillingDetails?.Name;
                await _context.SaveChangesAsync();
            }

            user.DefaultPaymentMethodId = request.PaymentMethodId;
            await _userManager.UpdateAsync(user);


            return Ok(new
            {
                success = true
            });

        }
    }

    public class CreateSetupRequest
    {
        public string StripeCustomerId { get; set; }
    }

    public class SavePaymentMethodRequest
    {
        public string PaymentMethodId { get; set; }
        public string StripeCustomerId { get; set; }
    }
}