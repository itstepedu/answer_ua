using System;
using System.Collections.Generic;
namespace AnswerUA.Models
{
    public class Orders
    {

        public int Id { get; set; }

        public string UserEmail { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Delivery { get; set; }

        public DateTime DeliveryDate { get; set; }

        public string Payment { get; set; }

        public decimal? SaleTotal { get; set; }

        public decimal? DeliveryPrice { get; set; }

        // Зв’язок з OrderItems
        public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
