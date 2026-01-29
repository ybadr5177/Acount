using System.ComponentModel.DataAnnotations;

namespace DashBoard.ViewModel.CozaMaster
{
    public class CategoryViewModel
    {
        public int id { get; set; }
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public string Picture { get; set; }
        public IFormFile PictureName { get; set; }

    }
}
