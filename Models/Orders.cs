using System;
using System.Collections.Generic;
namespace AnswerUA.Models
{
    public class Orders
    {
       
            public int Id { get; set; }

            // Email користувача, який зробив замовлення
            public string UserEmail { get; set; }

            // Дата створення замовлення
            public DateTime OrderDate { get; set; } = DateTime.Now;

             public DateTime TimeOrder { get; set; } = DateTime.Now;

            // Сума замовлення
            public decimal TotalAmount { get; set; }

            // Зв’язок з OrderItems
            public List<OrderItems> OrderItems { get; set; } = new List<OrderItems>();
    }
}
