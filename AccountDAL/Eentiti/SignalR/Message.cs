using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.SignalR
{
    public class Message:BaseEntity
    {
        public string SenderId { get; set; } // قد يكون AdminID أو ClientID
        public string RecipientId { get; set; }
        public string Content { get; set; } // نص الرسالة
        public string FilePath { get; set; } // مسار الملف/الصورة (إذا كانت رسالة مرفق)
        public DateTime Timestamp { get; set; }
        public bool IsFile { get; set; } // هل هذه رسالة نصية أم ملف؟
        public int ConversationId { get; set; } // لمعرفة سياق المحادثة
    }
}
