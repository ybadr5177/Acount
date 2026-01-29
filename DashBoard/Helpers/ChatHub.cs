using AccountDAL.Eentiti.SignalR;
using AccountDAL.Services;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DashBoard.Helpers
{
    public class ChatHub:Hub
    {
        private readonly IMessageService _messageService;
        private readonly ILogger<ChatHub> _logger;
        // قائمة لتسجيل معرّفات الاتصال الحالية للمدراء (للتوجيه السريع)
        // نستخدم ConcurrentDictionary لأن Hubs تتعامل مع اتصالات متزامنة
        private static readonly ConcurrentDictionary<string, string> ConnectedAdmins = new();

        // حقن خدمة الرسائل لحفظ البيانات
        public ChatHub(IMessageService messageService, ILogger<ChatHub> logger)
        {
            _messageService = messageService;
            _logger = logger;
        }
        public override async Task OnConnectedAsync()
        {
            // 🚨 الآن نعتمد على ID التوكن/Claims لتحديد هويته، بدلاً من Context.UserIdentifier مباشرةً
            var hubConnectedUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var connectionId = Context.ConnectionId;

            // 💡 بما أننا حذفنا شرط الدور، أي مستخدم مصادق عليه هو "المدير" هنا
            if (!string.IsNullOrEmpty(hubConnectedUserId))
            {
                // أ. المدراء: ينضمون إلى AdminGroup
                await Groups.AddToGroupAsync(connectionId, "AdminGroup");
                ConnectedAdmins.TryAdd(hubConnectedUserId, connectionId);
                _logger.LogInformation($"Admin Connected: {hubConnectedUserId} joined AdminGroup.");
            }
            else // العميل (غير مصادق عليه أو مصادق عليه ولكن ليس المدير)
            {
                // ب. العميل: يجب أن ينضم إلى مجموعته الخاصة عبر استدعاء JS (JoinClientGroup)
                // إذا كان العميل مصادق عليه، يمكننا محاولة الانضمام لمجموعته هنا
                // var clientUserId = Context.UserIdentifier;
                // if (!string.IsNullOrEmpty(clientUserId))
                // {
                //      await Groups.AddToGroupAsync(connectionId, $"Client_{clientUserId}");
                // }
                _logger.LogInformation($"Client Connected: Awaiting JoinClientGroup call.");
            }

            await base.OnConnectedAsync();
        }

        // 💡 إضافة دالة مساعدة للعميل للانضمام إلى مجموعته
        public async Task JoinClientGroup(string clientSenderId)
        {
            string groupName = $"Client_{clientSenderId}";
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            _logger.LogInformation($"Client Registered: {clientSenderId} joined their group {groupName}.");
        }


        // -----------------------------------------------------------
        // 2. إرسال الرسالة (تم تطبيق المنطق الجديد)
        // -----------------------------------------------------------
        public async Task SendMessage(string senderId, string recipientId, string messageContent, bool isFile, string filePath)
        {

            // 1. تحديد هوية المرسل (الآن نعتمد على ID التوكن/Claims)
            var hubConnectedUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // 🚨 تحديد المدير: إذا كان ID المرسل يطابق ID المستخدم المتصل والمصادق عليه، فهو المدير.
            bool isSenderAdmin = !string.IsNullOrEmpty(hubConnectedUserId) && senderId == hubConnectedUserId;

            // 2. تحديد ID العميل للمحادثة
            string clientUserId = isSenderAdmin ? recipientId : senderId;

            // 3. البحث عن المحادثة (يعتمد على ID العميل)
            var conversation = await _messageService.GetOrCreateConversationAsync(clientUserId);

            // 4. إنشاء نموذج الرسالة
            var message = new Message
            {
                SenderId = senderId,
                RecipientId = recipientId,
                Content = messageContent,
                FilePath = filePath,
                IsFile = isFile,
                Timestamp = DateTime.UtcNow,
                ConversationId = conversation.Id
            };

            // 5. حفظ الرسالة في قاعدة البيانات
            await _messageService.SaveMessageAsync(message);

            // 6. توجيه الرسالة عبر SignalR

            // 🚨 حزمة البيانات: تم إصلاح تنسيق الوقت ليصبح ISO 8601
            var messageData = new
            {
                SenderId = senderId,
                Content = messageContent,
                IsFile = isFile,
                FilePath = filePath,
                Timestamp = message.Timestamp.ToString("o"), // 💡 Fix: يستخدم تنسيق ISO 8601
                ConversationId = conversation.Id
            };

            if (!isSenderAdmin) // المرسل هو العميل
            {
                // أ. إذا كان المرسل عميلاً: أرسلها لكل المدراء المتصلين
                await Clients.Group("AdminGroup").SendAsync("ReceiveMessage", messageData);

                // إرسال نسخة للمرسل نفسه (Fallback)
                await Clients.Caller.SendAsync("ReceiveMessage", messageData);
            }
            else // المدير هو المرسل
            {
                // ب. إذا كان المرسل مديراً: أرسلها إلى العميل المحدد
                await Clients.Group($"Client_{recipientId}").SendAsync("ReceiveMessage", messageData);

                // 🚨 تحديث قائمة محادثات الداش بورد لدى المدراء الآخرين
                // نستخدم Clients.GroupExcept لإرسال الرسالة لجميع المدراء ما عدا المدير الحالي
                await Clients.GroupExcept("AdminGroup", Context.ConnectionId).SendAsync("ReceiveMessage", messageData);

                // 💡 لا حاجة لاستدعاء Clients.Caller.SendAsync هنا لأننا اعتمدنا على التحديث التفاؤلي في JS
            }
        }

        // -----------------------------------------------------------
        // 3. إدارة الفصل (عند قطع اتصال مستخدم)
        // -----------------------------------------------------------
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var hubConnectedUserId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(hubConnectedUserId))
            {
                // إزالة المدير من القائمة عند قطع الاتصال (يتوافق مع الكود القديم)
                ConnectedAdmins.TryRemove(hubConnectedUserId, out _);
            }
            return base.OnDisconnectedAsync(exception);
        }


        // -----------------------------------------------------------
        // 1. دول في حالت كان في roul)
        // -----------------------------------------------------------
        // -----------------------------------------------------------
        // 1. إدارة الاتصال (عند اتصال مستخدم جديد)
        // -----------------------------------------------------------

        //public override async Task OnConnectedAsync()
        //{
        //    var userId = Context.UserIdentifier; // معرّف المستخدم من نظام الـ Identity
        //    var connectionId = Context.ConnectionId; // معرّف الاتصال الحالي

        //    // 🚨 تحديد الدور: يعتمد على نظام الـ Identity لديك
        //    if (Context.User.IsInRole("Admin"))
        //    {
        //        // أ. المدراء: نضيفهم إلى مجموعة واحدة لتلقي رسائل جميع العملاء
        //        await Groups.AddToGroupAsync(connectionId, "AdminGroup");
        //        ConnectedAdmins.TryAdd(userId, connectionId);
        //    }
        //    else // العميل
        //    {
        //        // ب. العملاء: نضيف كل عميل إلى مجموعة خاصة به لتلقي رسائل المدراء
        //        await Groups.AddToGroupAsync(connectionId, $"Client_{userId}");
        //    }

        //    await base.OnConnectedAsync();
        //}

        // -----------------------------------------------------------
        // 2. إرسال الرسالة (يُستدعى من كود JavaScript)
        // -----------------------------------------------------------
        //public async Task SendMessage(string senderId, string recipientId, string messageContent, bool isFile, string filePath)
        //{
        //    // 1. البحث عن المحادثة
        //    var conversation = await _messageService.GetOrCreateConversationAsync(senderId);

        //    // 2. إنشاء نموذج الرسالة
        //    var message = new Message
        //    {
        //        SenderId = senderId,
        //        RecipientId = recipientId,
        //        Content = messageContent,
        //        FilePath = filePath,
        //        IsFile = isFile,
        //        Timestamp = DateTime.UtcNow,
        //        ConversationId = conversation.Id
        //    };

        //    // 3. حفظ الرسالة في قاعدة البيانات
        //    await _messageService.SaveMessageAsync(message);

        //    // 4. توجيه الرسالة عبر SignalR

        //    // حزمة البيانات التي سيتم إرسالها للعملاء في الوقت الفعلي
        //    var messageData = new
        //    {
        //        SenderId = senderId,
        //        Content = messageContent,
        //        IsFile = isFile,
        //        FilePath = filePath,
        //        Timestamp = message.Timestamp,
        //        ConversationId = conversation.Id
        //    };

        //    if (Context.User.IsInRole("Client"))
        //    {
        //        // أ. إذا كان المرسل عميلاً: أرسلها لكل المدراء المتصلين
        //        await Clients.Group("AdminGroup").SendAsync("ReceiveMessage", messageData);
        //    }
        //    else // المدير هو المرسل
        //    {
        //        // ب. إذا كان المرسل مديراً: أرسلها إلى العميل المحدد
        //        await Clients.Group($"Client_{recipientId}").SendAsync("ReceiveMessage", messageData);

        //        // 🚨 تحديث قائمة محادثات الداش بورد لدى المدراء الآخرين
        //        await Clients.GroupExcept("AdminGroup", Context.ConnectionId).SendAsync("UpdateAdminList", conversation.Id);
        //    }

        //    // 5. إرسال نسخة من الرسالة للمرسل نفسه لتظهر في واجهته على الفور (بدون انتظار إرسال الشبكة)
        //    await Clients.Caller.SendAsync("ReceiveMessage", messageData);
        //}

        // -----------------------------------------------------------
        // 3. إدارة الفصل (عند قطع اتصال مستخدم)
        // -----------------------------------------------------------
        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    if (Context.User.IsInRole("Admin"))
        //    {
        //        // إزالة المدير من القائمة عند قطع الاتصال
        //        ConnectedAdmins.TryRemove(Context.UserIdentifier, out _);
        //    }
        //    return base.OnDisconnectedAsync(exception);
        //}
    }
}
