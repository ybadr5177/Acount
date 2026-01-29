using AccountDAL.Eentiti.CozaStore;

namespace cozastore.ViewModel.CozaMaster
{
    public class ProductSizeViewModel
    {
        public int Id { get; set; }
        //public int ProductId { get; set; }
        //public Product Product { get; set; }

        public int DifferentSizeId { get; set; }
        //public DifferentSize Size { get; set; }
        public string SizeLabel { get; set; }

        //public bool IsActive { get; set; } = true;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public decimal? DiscountPrice { get; set; }
    }
}
