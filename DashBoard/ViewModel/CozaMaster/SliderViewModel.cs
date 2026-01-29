namespace DashBoard.ViewModel.CozaMaster
{
    public class SliderViewModel
    {
        public string Title_AR { get; set; }
        public string Title { get; set; }
        public string Description_AR { get; set; }
        public string Description { get; set; }

       
        public string Type { get; set; }


        public IFormFile Image { get; set; }
        public string ImagePath { get; set; }

        public string ExternalLink { get; set; }      
        public int? ProductId { get; set; }    
    }
}
