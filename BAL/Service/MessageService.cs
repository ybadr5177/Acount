using AccountDAL.Eentiti.SignalR;
using AccountDAL.Repositories;
using AccountDAL.Services;
using AccountDAL.Specifications.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Service
{
    public class MessageService : IMessageService
    {

        private readonly IUnitOfWork _unitOfWork;

        // حقن IUnitOfWork في الـ Constructor
        public MessageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // -------------------------------------------------------------------
        // 1. الحصول على المحادثة أو إنشاؤها (للاستخدام من قبل العميل/الـ ChatHub)
        // -------------------------------------------------------------------

        public async Task<Conversation> GetOrCreateConversationAsync(string clientId)
        {
            var conversationRepo = _unitOfWork.Repository<Conversation>();

            // استخدام Specification للبحث عن محادثة نشطة للعميل
            var spec = new ConversationByClientSpecification(clientId);

            // استخدام GetByIdWithSpecAsync (يفترض أنه يرجع عنصر واحد أو null)
            var conversation = await conversationRepo.GetByIdWithSpecAsync(spec);

            if (conversation != null)
            {
                return conversation;
            }

            // إذا لم توجد محادثة، نقوم بإنشاء واحدة جديدة
            var newConversation = new Conversation
            {
                ClientUserId = clientId,
                AdminUserId = "4bb9df56-c3f9-4a68-beb6-0dead18f8693",
                LastActivity = DateTime.UtcNow
            };

            await conversationRepo.AddAsync(newConversation);
            await _unitOfWork.Complete(); // حفظ المحادثة الجديدة في DB

            return newConversation;
        }

        // -------------------------------------------------------------------
        // 2. حفظ الرسالة وتحديث المحادثة (للاستخدام من قبل الـ ChatHub)
        // -------------------------------------------------------------------

        public async Task SaveMessageAsync(Message message)
        {
            // 1. جلب وتحديث سجل المحادثة (الريبو الأول)
            var conversationRepo = _unitOfWork.Repository<Conversation>();
            var conversation = await conversationRepo.GetByIdAsync(message.ConversationId);

            if (conversation != null)
            {
                conversation.LastActivity = message.Timestamp;
                conversationRepo.Update(conversation); // دالة Update متاحة في الريبو الخاص بك
            }

            // 2. حفظ الرسالة الجديدة (الريبو الثاني)
            var messageRepo = _unitOfWork.Repository<Message>();
            await messageRepo.AddAsync(message); // دالة AddAsync متاحة في الريبو الخاص بك

            // 3. حفظ كلتا العمليتين في معاملة واحدة
            await _unitOfWork.Complete();
        }

        // -------------------------------------------------------------------
        // 3. استرجاع سجل المحادثة (للتحميل الأولي في الداش بورد)
        // -------------------------------------------------------------------

        public async Task<List<Message>> GetConversationMessagesAsync(int conversationId)
        {
            var messageRepo = _unitOfWork.Repository<Message>();

            // استخدام Specification لجلب الرسائل بالمعرف وترتيبها زمنياً
            var spec = new MessagesByConversationSpecification(conversationId);

            // استخدام GetAllWithSpecAsync لجلب قائمة الرسائل
            var messages = await messageRepo.GetAllWithSpecAsync(spec);

            return messages.ToList();
        }

        // -------------------------------------------------------------------
        // 4. دالة إضافية مفيدة: جلب كل المحادثات النشطة (لقائمة الداش بورد)
        // -------------------------------------------------------------------

        // ستحتاج لإضافة هذه الدالة لـ IMessageService لتكون متوافقة
        public async Task<IReadOnlyList<Conversation>> GetAllActiveConversationsAsync()
        {
            var conversationRepo = _unitOfWork.Repository<Conversation>();

            // هنا نستخدم GetAllAsync ونعتمد على الترتيب من جانب الكود أو نستخدم Specification أخرى
            // بما أن GetAllAsync متوفرة، سنستخدمها ونرتبها في الـ Controller إذا لم يكن الترتيب جزءاً من Spec
            // أو يمكنك إضافة Specification لـ OrderBy
            return await conversationRepo.GetAllAsync();
        }
    }
}

