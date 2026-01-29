using AccountDAL.Services;
using cozastore.ViewModel.SignalR;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace cozastore.Controllers
{
    public class ClientChatController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMessageService _messageService;

        // حقن خدمة الرسائل
        public ClientChatController(IMessageService messageService, HttpClient httpClient, IHttpClientFactory httpClientFactory)
        {
            _messageService = messageService;
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
        }

        // -----------------------------------------------------------
        // Action: Index (عرض واجهة الدردشة للعميل)
        // -----------------------------------------------------------

        public async Task<IActionResult> Index()
        {
           

            // 1. الحصول على معرّف العميل الحالي
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(clientId))
            {
                // يجب أن لا يحدث هذا إذا كان [Authorize] يعمل بشكل صحيح
                return Unauthorized();
            }

            // 2. الحصول على المحادثة أو إنشاؤها
            // هذا يضمن أن لدينا ConversationId لاستخدامه لاحقاً في الـ View والـ SignalR
            var conversation = await _messageService.GetOrCreateConversationAsync(clientId);

            // 3. جلب سجل الرسائل القديم
            var messages = await _messageService.GetConversationMessagesAsync(conversation.Id);

            // 4. تجميع البيانات للـ View (يمكن استخدام ViewModel)
            var viewModel = new ClientChatViewModel // نفترض وجود ViewModel
            {
                ConversationId = conversation.Id,
                SenderId = clientId,
                Messages = messages
            };

            return View(viewModel);
        }
    }
}
