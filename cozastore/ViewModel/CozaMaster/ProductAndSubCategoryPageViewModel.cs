using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Specifications.ProductsSpecification;

namespace cozastore.ViewModel.CozaMaster
{
    public class ProductAndSubCategoryPageViewModel
    {
        public ProductSpecParams Filter { get; set; } = new();
        public List<ProductDisplayViewModel> Products { get; set; } = new();
        public IReadOnlyList<SubCategory>? SubCategories { get; set; }
    }
}
