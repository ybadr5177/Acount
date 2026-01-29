using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class Feedback:BaseEntity
    {
        public string? Email { get; set; }  // البريد هيتم جلبه من Identity
        [Required]
        public string Subject { get; set; } // عنوان الشكوى أو الاقتراح
        [Required]
        public string Message { get; set; }
    }
}
