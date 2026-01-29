using AccountDAL.Eentiti.CozaStore;

namespace cozastore.ViewModel.CozaMaster
{
    public class DetailsProductDisplayViewModel
    {
        public string Address { get; set; }
        public string description { get; set; }

        public List<string> Picture { get; set; }
        public List<ProductSizeViewModel> ProductSize { get; set; } = new();
        public int? SelectedProductSizeId { get; set; }

        public decimal? SelectedPrice { get; set; }

    }
}
