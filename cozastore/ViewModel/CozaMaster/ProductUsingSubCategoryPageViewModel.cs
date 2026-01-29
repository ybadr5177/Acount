using AccountDAL.Specifications.ProductsSpecification;

namespace cozastore.ViewModel.CozaMaster
{
    public class ProductUsingSubCategoryPageViewModel
    {
        public ProductSpecParams Filter { get; set; } = new();
        public List<ProductDisplayViewModel> Products { get; set; } = new();
    }
}
