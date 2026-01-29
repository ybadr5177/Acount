using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Specifications.ProductsSpecification;

namespace cozastore.ViewModel.CozaMaster
{
    public class ProductPageViewModel
    {
        public ProductSpecParams Filter { get; set; } = new();
        public List<ProductDisplayViewModel> Products { get; set; } = new();
        public IReadOnlyList<Category>? Categories { get; set; } 

    }
}
