using Microsoft.AspNetCore.Mvc.Rendering;

namespace DashBoard.ViewModel.CozaMaster
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public int CategoryId { get; set; }
        public string Picture { get; set; }
        public IFormFile PictureName { get; set; }
        public List<CategoryViewModel> Categories { get; set; } = new();

        public string? CategoryName { get; set; }

    }
}
