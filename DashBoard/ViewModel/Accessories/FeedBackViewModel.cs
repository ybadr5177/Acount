using System.ComponentModel.DataAnnotations;

namespace DashBoard.ViewModel.Accessories
{
    public class FeedBackViewModel
    {
        public int? Id { get; set; }
        public string? Email { get; set; }  // البريد هيتم جلبه من Identity
        [Required]
        public string Subject { get; set; } // عنوان الشكوى أو الاقتراح
        [Required]
        public string Message { get; set; }
    }
}
