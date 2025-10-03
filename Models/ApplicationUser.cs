using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace AnswerUA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public DateTime DateOfBirth { get; set; }
        public decimal AccumulatedPoints { get; set; }
        public decimal PermanentDiscount { get; set; }

        [NotMapped]
        public string Role { get; set; } = string.Empty;

        public string? StripeCustomerId { get; set; }
        public string? DefaultPaymentMethodId { get; set; }

        public List<Address>? Addresses { get; set; }

        public List<PaymentMethod>? PaymentMethods { get; set; }
    }
}