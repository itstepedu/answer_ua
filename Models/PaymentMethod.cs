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

        public string? Brand { get; set; }
        public string? Last4 { get; set; }

        public int? ExpMonth { get; set; }
        public int? ExpYear { get;  set;}
        public string? CardHolderName { get; set; } 
        public bool IsDefault { get; set; }      
    }
}