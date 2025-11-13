using System;
using System.Collections.Generic;
using answer_ua.Areas.Identity.Pages.Account.Manage;
namespace AnswerUA.Models
{
    public class Orders
    {

        public int Id { get; set; }

        // Email користувача, який зробив замовлення
        public string UserEmail { get; set; }

        // Дата створення замовлення
        public DateTime OrderDate { get; set; } = DateTime.Now;

        //public DateTime TimeOrder { get; set; } = DateTime.Now;

        // Сума замовлення
        public decimal TotalAmount { get; set; }

        // Зв’язок з OrderItems
        public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();

        // Спосіб доставки
        public string? Delivery { get; set; }

        // Дата прибуття замовлення
        public DateTime DeliveryDate { get; set; }

        // Спосіб оплати
        public string Payment { get; set; }

        // Сума знижки
        public decimal? SaleTotal { get; set; } = 0;

        // Сума замовлення
        public decimal? DeliveryPrice { get; set; } = 0;
    }
}
