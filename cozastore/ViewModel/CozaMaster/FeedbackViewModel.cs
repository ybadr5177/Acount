using System.ComponentModel.DataAnnotations;

namespace cozastore.ViewModel.CozaMaster
{
    public class FeedbackViewModel
    {
        public string? Email { get; set; }  // البريد هيتم جلبه من Identity
        [Required]
        public string Subject { get; set; } // عنوان الشكوى أو الاقتراح
        [Required]
        public string Message { get; set; }
    }
}
