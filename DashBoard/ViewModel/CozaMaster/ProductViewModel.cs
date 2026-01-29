using AccountDAL.Eentiti.CozaStore;

namespace DashBoard.ViewModel.CozaMaster
{
    public class ProductViewModel
    {
        public string Address_En { get; set; }
        public string Address_Ar { get; set; }
        public string ProductSizesJson { get; set; }

        public string description_En { get; set; }
        public string description_Ar { get; set; }
        public string Type { get; set; }
        public bool IsActive { get; set; } = true; //  If you want to show or hide the product(لو عايز اظهر او اخفي المنتج)

        public int? CategoryId { get; set; }
        public List<CategoryViewModel>? Categorys { get; set; }


        public int? SubCategoryId { get; set; }
        public List<SubCategoryViewModel>? SubCategorys { get; set; }
        public decimal? BasePrice { get; set; }
        public decimal? Discountrate { get; set; }
        public decimal? BaseDiscountPrice { get; set; }
        //public string ImageName { get; set; }
        public List<string> ImageName { get; set; }
        public List<IFormFile> PictureName { get; set; }
        public List<ProductSizeEntryViewModel> ProductSizes { get; set; } = new List<ProductSizeEntryViewModel>();
       
    }
}
