using cozastore.ViewModel.Accessories;
using cozastore.ViewModel.CozaMaster;

namespace cozastore.ViewModel
{
    public class SocialLinksAndCatagreyViewModel
    {
        public ContactUSdbViewModel contactUSdb { get; set; }
        public IEnumerable<CategoryViewModel> Category { get; set; }
    }
}
