using AnswerUA.Models;

namespace AnswerUA.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }     
    }

}