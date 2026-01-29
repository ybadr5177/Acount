using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class Favorites:BaseEntity
    {
       
        public string UserEmail { get; set; } // البريد الإلكتروني للمستخدم

        public int ProductId { get; set; }  // Foreign Key للمنتج
        public Product Product { get; set; } // Navigation Property

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
