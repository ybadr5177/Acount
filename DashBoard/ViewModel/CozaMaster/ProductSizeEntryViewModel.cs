namespace DashBoard.ViewModel.CozaMaster
{
    public class ProductSizeEntryViewModel
    {
        public int Id { get; set; }
        public int DifferentSizeId { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal? Discountrate { get; set; }

        public int Stock { get; set; }
        public string SizeLabel { get; set; }
    }
}
