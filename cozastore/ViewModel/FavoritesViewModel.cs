using cozastore.ViewModel.CozaMaster;

namespace cozastore.ViewModel
{
    public class FavoritesViewModel
    {
        public string UserEmail { get; set; } // البريد الإلكتروني للمستخدم

        public int ProductId { get; set; }  // Foreign Key للمنتج
        public ProductDisplayViewModel Product { get; set; } // Navigation Property

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
