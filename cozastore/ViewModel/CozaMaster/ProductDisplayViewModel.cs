using AccountDAL.Eentiti.CozaStore;

namespace cozastore.ViewModel.CozaMaster
{
    public class ProductDisplayViewModel
    {
        public int Id { get; set; }


        public string Address { get; set; }
        public string description { get; set; }
        
        public List<Category>? Categories { get; set; }

        public int? SubCategorysId { get; set; }

        public string? CategoryName { get; set; }
        public string? SubCategorysName { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }
        public decimal BaseDiscountPrice { get; set; }
        public List<ProductSizeViewModel>? ProductSize { get; set; } = new();
        public List<string> Picture { get; set; }
    }
}
