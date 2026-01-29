using System.ComponentModel.DataAnnotations;

namespace cozastore.ViewModel
{
    public class CustomerBasketViewModel
    {
        [Required]
        public string Id { get; set; }
        public List<BasketItemViewModel> Items { get; set; } = new List<BasketItemViewModel>();
        public int DeliveryCostId { get; set; }
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
