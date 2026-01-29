namespace DashBoard.ViewModel.CozaMaster
{
    public class productDisplayViewModel
    {
        public int id { get; set; }
        public string Name { get; set; }
        public string description { get; set; }
        public string? SubCategoryName { get; set; }

        public string? CategoryName { get; set; }

        public decimal? BasePrice { get; set; }

        public List<string>? Picture { get; set; }
        public string FirstPicture => Picture != null && Picture.Any() ? Picture[0] : "default-image.png";
        public string? FirstImage { get; set; }

        public bool IsActive { get; set; } 


    }
}
