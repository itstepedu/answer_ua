namespace AnswerUA.Models
{
    public class OrderItems
    {
        public int Id { get; set; }

        // Посилання на товар
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // Посилання на замовлення
        public int OrdersId { get; set; }
        public Orders Orders { get; set; }

        // Кількість товару в замовленні
        public int Quantity { get; set; }

        // Ціна на момент покупки
        public decimal Price { get; set; }
    }
}
