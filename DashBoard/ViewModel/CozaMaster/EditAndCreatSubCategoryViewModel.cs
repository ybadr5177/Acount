namespace DashBoard.ViewModel.CozaMaster
{
    public class EditAndCreatSubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public int CategoryId { get; set; }
        public string? Picture { get; set; }
        public IFormFile PictureName { get; set; }
    }
}
