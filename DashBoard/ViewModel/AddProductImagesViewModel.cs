namespace DashBoard.ViewModel
{
    public class AddProductImagesViewModel
    {
        public IFormFile Image { get; set; }
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }
    }
}
