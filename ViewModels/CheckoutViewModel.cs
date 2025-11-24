using AnswerUA.Models;

namespace AnswerUA.ViewModels
{
    public class CheckoutViewModel
    {

        public ApplicationUser User { get; set; }
        public List<CartItem> Cart { get; set; }
        public List<Address> Addresses { get; set; }
        public List<PaymentMethod> PaymentMethods { get; set; }
    }
    public class CheckoutOrderInput
    {
        public int AddressId { get; set; }
        public string DeliveryMethod { get; set; }
        public string PaymentMethod { get; set; }
    }
}
