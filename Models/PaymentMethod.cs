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
        public enum PaymentMethodType { Card, GooglePay, ApplePay, PayPal }
        public PaymentMethodType MethodType { get; set; }    
        public string? CardNumber { get; set; }   
        public string? CardHolderName { get; set; } 
        public DateTime? ExpirationDate { get; set; } 
        public string? WalletEmail { get; set; }  
        public bool IsDefault { get; set; }      
    }
}