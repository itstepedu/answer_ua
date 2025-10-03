using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AnswerUA.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public ApplicationUser? User { get; set; }

        public string? PaymentMethodId { get; set; }
        public string? Brand { get; set; }
        public string? Last4 { get; set; }
        public long? ExpMonth { get; set; }
        public long? ExpYear { get; set; }
        public string? CardHolderName { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}