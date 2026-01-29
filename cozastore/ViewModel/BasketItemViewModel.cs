using AccountDAL.Eentiti.CozaStore;
using System.ComponentModel.DataAnnotations;

namespace cozastore.ViewModel
{
    public class BasketItemViewModel
    {
       
        public int Id { get; set; }


        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public List<Category>? Categories { get; set; }

        public int? SubCategorysId { get; set; }

        public string? CategoryName { get; set; }
        public string? SubCategorysName { get; set; }

        [Required]
        [Range(0.1, double.MaxValue, ErrorMessage = "Price Must be greater than zero!!")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Size { get; set; }
        public decimal BaseDiscountPrice { get; set; }
        public string PictureUrl { get; set; }
    }
}
