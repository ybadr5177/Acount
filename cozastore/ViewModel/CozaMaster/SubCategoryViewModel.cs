namespace cozastore.ViewModel.CozaMaster
{
    public class SubCategoryViewModel
    {
        public int Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public int CategoryId { get; set; }
        public string Picture { get; set; }
      
        public List<CategoryViewModel> Categories { get; set; } = new();
    }
}
