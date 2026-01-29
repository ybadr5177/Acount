using AccountDAL.Eentiti.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Services
{
    public interface IMessageService
    {

        // دالة لحفظ الرسالة المرسلة
        Task SaveMessageAsync(Message message);

        // دالة لاسترداد سجل المحادثة (للتحميل الأولي للصفحة)
        Task<List<Message>> GetConversationMessagesAsync(int conversationId);

        // دالة للحصول على المحادثة النشطة أو إنشائها لعميل جديد
        Task<Conversation> GetOrCreateConversationAsync(string clientId);
        // 💡 الدالة الجديدة التي سببت الخطأ:
        Task<IReadOnlyList<Conversation>> GetAllActiveConversationsAsync();
    }
}
